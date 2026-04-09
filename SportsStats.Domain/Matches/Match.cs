using SportsStats.Domain.Common;
using SportsStats.Domain.Matches.Goals;
using SportsStats.Domain.Tournaments.Rules;
using SportsStats.Domain.Matches;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SportsStats.Domain.Shared.Enums;

namespace SportsStats.Domain.Matches
{
	public class Match : BaseEntity, IAggregateRoot
	{
		private readonly List<GoalEvent> _goals = new();
		private List<int> _homeTeamRoster = new();
		private List<int> _awayTeamRoster = new();
		private readonly TournamentRules _rules;
		public int HomeTeamId { get; private set; }
		public int AwayTeamId { get; private set; }
		public IReadOnlyList<int> HomeTeamRoster => _homeTeamRoster;
		public IReadOnlyList<int> AwayTeamRoster => _awayTeamRoster;
		public DateTime? StartedAt { get; private set; }
		public DateTime? FinishedAt { get; private set; }
		public int TournamentId { get; private set; }
		public MatchStatus Status { get; private set; } = MatchStatus.Waiting;
		public int HomeTeamScore { get; private set; } = 0;
		public int AwayTeamScore { get; private set; } = 0;
		public MatchWinType? HomeTeamWinType { get; private set; }
		public MatchWinType? AwayTeamWinType { get; private set; }
		public bool IsOvertime { get; private set; }
		public IReadOnlyList<GoalEvent> Goals => _goals;
		public TournamentRules Rules => _rules;
		private Match() { }
		public Match(int tournamentId, int homeTeamId, int awayTeamId, TournamentRules rules)
		{
			if (homeTeamId == awayTeamId)
				throw new ArgumentException("Команда не может играть сама с собой");

			TournamentId = tournamentId;
			HomeTeamId = homeTeamId;
			AwayTeamId = awayTeamId;

			_rules = rules ?? throw new ArgumentNullException();
		}
		public void Start(DateTime startedAt)
		{
			if (Status != MatchStatus.Waiting)
				throw new ArgumentException("Нельзя начать матч, который уже не в ожидании");

			StartedAt = startedAt;
			Status = MatchStatus.InProgress;
		}

		public void Finish(DateTime finishedAt)
		{
			if (Status != MatchStatus.InProgress)
				throw new ArgumentException("Нельзя завершить матч, который ещё не начат или уже закончен");
			if (HomeTeamScore == AwayTeamScore && !_rules.MatchDurationRules.IsDrawPossible)
				throw new ArgumentException("Нельзя завершить матч с ничейным счётом, когда ничья запрещена правилами");

			FinishedAt = finishedAt;
			Status = MatchStatus.Finished;

			if (HomeTeamScore > AwayTeamScore)
			{
				if (IsOvertime)
				{
					HomeTeamWinType = MatchWinType.OT_WIN;
					AwayTeamWinType = MatchWinType.OT_LOSS;
				}
				else
				{
					HomeTeamWinType = MatchWinType.REGULATION_WIN;
					AwayTeamWinType = MatchWinType.REGULATION_LOSS;
				}
			}
			else if (HomeTeamScore < AwayTeamScore)
			{
				if (IsOvertime)
				{
					HomeTeamWinType = MatchWinType.OT_LOSS;
					AwayTeamWinType = MatchWinType.OT_WIN;
				}
				else
				{
					HomeTeamWinType = MatchWinType.REGULATION_LOSS;
					AwayTeamWinType = MatchWinType.REGULATION_WIN;
				}
			}
			else
			{
				HomeTeamWinType = MatchWinType.DRAW;
				AwayTeamWinType = MatchWinType.DRAW;
			}
		}


		protected bool IsPlayerInHomeRoster(int playerId)
		{
			return HomeTeamRoster.Contains(playerId);
		}

		protected bool IsPlayerInAwayRoster(int playerId)
		{
			return AwayTeamRoster.Contains(playerId);
		}

		protected bool IsPlayerOnRoster(int playerId, int? teamId = null)
		{
			return (teamId == null && (IsPlayerInAwayRoster(playerId) || IsPlayerInHomeRoster(playerId)))
				|| (HomeTeamId == teamId && IsPlayerInHomeRoster(playerId))
				|| (AwayTeamId == teamId && IsPlayerInAwayRoster(playerId));
		}

		protected bool IsTeamInMatch(int teamId)
		{
			return teamId == HomeTeamId || teamId == AwayTeamId;
		}

		public bool IsMatchInProgress()
		{
			return Status == MatchStatus.InProgress;
		}
		public bool IsMatchFinished()
		{
			return Status == MatchStatus.Finished;
		}
		public bool IsMatchWaiting()
		{
			return Status == MatchStatus.Waiting;
		}


		public GoalEvent AddGoal(int scoringTeamId, int goalScorerId, int period, int time, DateTime scoringMoment)
		{
			ValidateGoal(scoringTeamId, goalScorerId, period, time);

			GoalEvent goal = CreateGoal(scoringTeamId, goalScorerId, period, time);
			_goals.Add(goal);

			if (scoringTeamId == HomeTeamId) HomeTeamScore++;
			else AwayTeamScore++;

			if (_rules.MatchDurationRules.IsOvertimePeriod(period))
				IsOvertime = true;

			if (_rules.MatchDurationRules.DoesGoalEndMatch(period))
				Finish(scoringMoment);
			return goal;
		}

		protected void ValidateGoal(int scoringTeamId, int goalScorerId, int period, int time)
		{
			if (!IsMatchInProgress())
				throw new ArgumentException("Нельзя добавить гол, матчу, который сейчас не идёт");

			if (!IsTeamInMatch(scoringTeamId))
				throw new ArgumentException("Нельзя назначить забившей команду, которая не учавствует в матче");
			if (!IsPlayerOnRoster(goalScorerId, scoringTeamId))
				throw new ArgumentException("Нельзя назначить автором гола игрока, которого нет в заявке за эту команду");

			ValidateGoalTiming(period, time);
		}

		protected void ValidateGoalTiming(int period, int time)
		{
			if (!_rules.MatchDurationRules.IsValidPeriod(period))
				throw new ArgumentException("Проверьте значение периода, по правилам такого периода не существует");
			if (!_rules.MatchDurationRules.IsValidTimeInPeriod(period, time))
				throw new ArgumentException("Проверьте время, данный период не может иметь такое время.");
			if (_rules.MatchDurationRules.IsOvertimePeriod(period) && HomeTeamScore != AwayTeamScore)
				throw new ArgumentException("Нельзя добавить гол в овертайме, если у команд разный счёт");
		}

		protected GoalEvent CreateGoal(int scoringTeamId, int goalScorerId, int period, int time)
		{
			return new GoalEvent(scoringTeamId, goalScorerId, period, time);
		}

		protected GoalEvent GetGoalEventById(int goalId)
		{
			GoalEvent goal = _goals.SingleOrDefault(goal => goal.Id == goalId);
			if (goal == null)
				throw new ArgumentException("Гол с данным Id не содержится в событиях этого матча");

			return goal;
		}

		// TODO: Вынести управление голами в GoalsCollection
		// Проблема: Match обрастает методами для работы с GoalEvent
		// Решение: создать GoalsCollection с методами Add, UpdateAssists, UpdateStrength, UpdateNetType
		// Match будет делегировать вызовы или предоставлять доступ через свойство Goals
		// Приоритет: после MVP (когда потребуется частое редактирование голов)
		public void FillGoalDetails(int goalId, int? firstAssistId, int? secondAssistId,
									GoalStrengthType strengthType, GoalNetType? netType)
		{
			GoalEvent goal = GetGoalEventById(goalId);
			if (firstAssistId.HasValue && !IsPlayerOnRoster(firstAssistId.Value, goal.ScoringTeamId))
				throw new ArgumentException("Этот игрок не заявлен за команду, которая забила гол");
			if (secondAssistId.HasValue && !IsPlayerOnRoster(secondAssistId.Value, goal.ScoringTeamId))
				throw new ArgumentException("Этот игрок не заявлен за команду, которая забила гол");

			goal.SetAssists(firstAssistId, secondAssistId);
			goal.SetNetType(netType);
			goal.SetStrengthType(strengthType);
		}

		public void AddPlayerToRoster(int playerId, int teamId)
		{
			if (!IsTeamInMatch(teamId))
				throw new ArgumentException("Нельзя заявить игрока за команду, которая не учавствует в матче");
			if (IsPlayerOnRoster(playerId))
				throw new ArgumentException("Нельзя добавить игрока дважды");
			if (!IsMatchWaiting())
				throw new ArgumentException("Нельзя добавить игрока, после начала матча");

			if (teamId == HomeTeamId)
			{
				_homeTeamRoster.Add(playerId);
			}
			else
			{
				_awayTeamRoster.Add(playerId);
			}

		}

	}
}
