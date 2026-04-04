using SportsStats.Domain.Common;
using SportsStats.Domain.Players;
using SportsStats.Domain.Shared;
using SportsStats.Domain.Tournaments;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Timers;

namespace SportsStats.Domain.Matches
{
	public abstract class MatchBase<TGoal> : BaseEntity, IAggregateRoot where TGoal : GoalEventBase
	{
		public int HomeTeamId { get; private set; }
		public int AwayTeamId { get; private set; }
		public List<int> HomeTeamRoster { get; private set; }
		public List<int> AwayTeamRoster { get; private set; }
		public DateTime? StartedAt { get; private set; }
		public DateTime? FinishedAt { get; private set; }
		public int HomeScore { get; protected set; }
		public int AwayScore { get; protected set; }
		public int TournamentId { get; private set; }
		public MatchStatuses Status { get; private set; } = MatchStatuses.Waiting;


		private IExistenceChecker _teamExistenceChecker;
		private IExistenceChecker _tournamentExistenceChecker;
		private IPlayerDataProvider _playerDataProvider;
		private ITournamentDataProvider _tournamentDataProvider;
		protected TimeBasedTournamentRules _timeBasedTournamentRules;
		private List<TGoal> _goals;

		public MatchBase(int homeTeamId, int awayTeamId, int tournamentId,
						 IExistenceChecker teamExistenceChecker,
						 IExistenceChecker tournamentExistenceChecker,
						 IPlayerDataProvider playerDataProvider,
						 ITournamentDataProvider tournamentDataProvider)
		{
			_teamExistenceChecker = teamExistenceChecker;
			_tournamentExistenceChecker = tournamentExistenceChecker;
			_playerDataProvider = playerDataProvider;
			_tournamentDataProvider = tournamentDataProvider;

			if (_teamExistenceChecker == null)
				throw new ArgumentException("Не установлен чеккер id команды");
			if (_tournamentExistenceChecker == null)
				throw new ArgumentException("Не установлен чеккер id турнира");
			if (!_teamExistenceChecker.Exists(homeTeamId))
				throw new ArgumentException("Не существует домашней команды");
			if (!_teamExistenceChecker.Exists(awayTeamId))
				throw new ArgumentException("Не существует гостевой команды");
			if (!_tournamentExistenceChecker.Exists(tournamentId))
				throw new ArgumentException("Не существует такого турнира");
			if (_playerDataProvider == null)
				throw new ArgumentException("Не установлен провайдер для игрока");

			HomeTeamId = homeTeamId;
			AwayTeamId = awayTeamId;
			TournamentId = tournamentId;
			_timeBasedTournamentRules = _tournamentDataProvider.GetTimeBasedTournamentRules(TournamentId);
		}
		public void Start()
		{
			if (Status != MatchStatuses.Waiting)
				throw new ArgumentException("Нельзя начать матч, который уже не в ожидании");

			StartedAt = DateTime.Now;
			Status = MatchStatuses.InProgress;
		}
		public void Finish()
		{
			if (Status != MatchStatuses.InProgress)
				throw new ArgumentException("Нельзя завершить матч, который ещё не начат или уже закончен");

			FinishedAt = DateTime.Now;
			Status = MatchStatuses.Finished;
		}
		public void AddGoal(int scoringTeamId, int goalScorerId, int period, int time)
		{
			if (!IsMatchInProgress())
				throw new ArgumentException("Нельзя добавить гол, матчу, который сейчас не идёт");

			if (!IsPlayerInMatch(goalScorerId))
				throw new ArgumentException("Нельзя назначить автором гола игрока, который не учавствует в матче");
			if (!IsTeamInMatch(scoringTeamId))
				throw new ArgumentException("Нельзя назначить забившей команду, которая не учавствует в матче");

			ValidateGoalTiming(period, time);

			TGoal goal = CreateGoal(scoringTeamId, goalScorerId, period, time);
			_goals.Add(goal);

			if (IsGameEndingGoal(goal))
				Finish();

		}
		protected bool IsPlayerInHomeTeam(int playerId)
		{
			return _playerDataProvider.GetTeamId(playerId) == HomeTeamId;
		}
		protected bool IsPlayerInAwayTeam(int playerId)
		{
			return _playerDataProvider.GetTeamId(playerId) == AwayTeamId;
		}
		protected bool IsPlayerInMatch(int playerId)
		{
			return IsPlayerInAwayTeam(playerId) || IsPlayerInHomeTeam(playerId);
		}
		protected bool IsTeamInMatch(int teamId)
		{
			return teamId == HomeTeamId || teamId == AwayTeamId;
		}
		protected bool IsPlayerOnMatchRoster(int playerId, int? teamId = null)
		{
			return (teamId == null && (HomeTeamRoster.Contains(playerId) || AwayTeamRoster.Contains(playerId)))
				|| (HomeTeamId == teamId && HomeTeamRoster.Contains(playerId))
				|| (AwayTeamId == teamId && AwayTeamRoster.Contains(playerId));
		}
		protected virtual void ValidateGoalTiming(int period, int time)
		{
			if (period <= 0)
				throw new ArgumentException("Период должен быть положительным");
			if (time < 0)
				throw new ArgumentException("Время не может быть отрицательным");



			if (period > _timeBasedTournamentRules.PeriodsCount)
			{
				if (!_timeBasedTournamentRules.HasOvertime)
					throw new ArgumentException($"Нельзя добавить гол в период {period}, так как в матче " +
												$"лишь {_timeBasedTournamentRules.PeriodsCount} периодов в основное время " +
												$"а овертаймы не предусмотрены");

				if (_timeBasedTournamentRules.OvertimesCount.HasValue
					&& period > _timeBasedTournamentRules.OvertimesCount + _timeBasedTournamentRules.PeriodsCount)
					throw new ArgumentException($"Нельзя добавить гол в период {period}, так как в матче " +
												$"лишь {_timeBasedTournamentRules.PeriodsCount} периодов в основное время " +
												$"и максимум {_timeBasedTournamentRules.OvertimesCount} периодов овертайма");

				if (_timeBasedTournamentRules.OvertimeDurationSeconds.HasValue && time > _timeBasedTournamentRules.OvertimeDurationSeconds.Value)
					throw new ArgumentException($"Нельзя добавить гол на {time} секунде овертайма, " +
												$"потому что длительность овертаймов {_timeBasedTournamentRules.OvertimeDurationSeconds}");
			}
			else if (time > _timeBasedTournamentRules.PeriodDurationSeconds)
				throw new ArgumentException($"Нельзя добавить гол, забитый на {time} секунде основного периода, " +
											$"так как его продолжительность {_timeBasedTournamentRules.PeriodDurationSeconds} секунд");
		}
		protected abstract TGoal CreateGoal(int scoringTeamId, int goalScorerId, int period, int time);
		protected TGoal GetGoalById(int goalId)
		{
			TGoal goal = _goals.SingleOrDefault(goal => goal.Id == goalId);
			if (goal == null)
				throw new ArgumentException("Гол с данным Id не содержится в событиях этого матча");

			return goal;
		}
		public bool IsMatchInProgress()
		{
			return Status == MatchStatuses.InProgress;
		}
		protected bool IsGameEndingGoal(TGoal goal)
		{
			return goal.Period > _timeBasedTournamentRules.PeriodsCount && _timeBasedTournamentRules.SuddenDeathOvertime;
		}
	}
}
