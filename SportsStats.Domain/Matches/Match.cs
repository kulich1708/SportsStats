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
		public MatchTeam HomeTeam { get; private set; }
		public MatchTeam AwayTeam { get; private set; }
		public DateTime ScheduledAt { get; private set; }
		public DateTime? StartedAt { get; private set; }
		public DateTime? FinishedAt { get; private set; }
		public int TournamentId { get; private set; }
		public MatchStatus Status { get; private set; } = MatchStatus.Waiting;
		public bool IsOvertime { get; private set; }
		public IReadOnlyList<GoalEvent> Goals => _goals;
		public TournamentRules Rules { get; private set; }
		private Match() { }
		public Match(int tournamentId, int homeTeamId, int awayTeamId, TournamentRules rules, DateTime scheduledAt)
		{
			if (homeTeamId == awayTeamId)
				throw new ArgumentException("Команда не может играть сама с собой");

			TournamentId = tournamentId;
			HomeTeam = new(homeTeamId);
			AwayTeam = new(awayTeamId);

			Rules = rules ?? throw new ArgumentNullException();
			ScheduledAt = scheduledAt;
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
			if (StartedAt > finishedAt)
				throw new ArgumentException($"Нельзя завершить матч {finishedAt}, так как он был начат лишь {StartedAt}");

			if (HomeTeam.Score == AwayTeam.Score && !Rules.MatchTimeRules.IsDrawPossible)
				throw new ArgumentException("Нельзя завершить матч с ничейным счётом, когда ничья запрещена правилами");

			FinishedAt = finishedAt;
			Status = MatchStatus.Finished;

			SetWinTypes();
		}
		private void SetWinTypes()
		{
			if (HomeTeam.Score == AwayTeam.Score)
			{
				HomeTeam.SetWinType(MatchWinType.DRAW);
				AwayTeam.SetWinType(MatchWinType.DRAW);
				return;
			}

			var isHomeWin = HomeTeam.Score > AwayTeam.Score;

			if (IsOvertime)
			{
				HomeTeam.SetWinType(isHomeWin ? MatchWinType.OT_WIN : MatchWinType.OT_LOSS);
				AwayTeam.SetWinType(isHomeWin ? MatchWinType.OT_LOSS : MatchWinType.OT_WIN);
			}
			else
			{
				HomeTeam.SetWinType(isHomeWin ? MatchWinType.REGULATION_WIN : MatchWinType.REGULATION_LOSS);
				AwayTeam.SetWinType(isHomeWin ? MatchWinType.REGULATION_LOSS : MatchWinType.REGULATION_WIN);
			}
		}
		private MatchTeam GetTeamById(int teamId)
		{
			if (teamId == HomeTeam.Id)
				return HomeTeam;
			else if (teamId == AwayTeam.Id)
				return AwayTeam;

			throw new ArgumentException("Команада не учавствует в этом матче");
		}
		private bool IsPlayerInRoster(int playerId, int? teamId = null)
		{
			if (!teamId.HasValue && (HomeTeam.IsPlayerInRoster(playerId) || AwayTeam.IsPlayerInRoster(playerId)))
				return true;
			if (teamId.HasValue && GetTeamById(teamId.Value).IsPlayerInRoster(playerId))
				return true;

			return false;
		}

		private bool IsTeamInMatch(int teamId) => teamId == HomeTeam.Id || teamId == AwayTeam.Id;
		public bool IsMatchInProgress() => Status == MatchStatus.InProgress;
		public bool IsMatchFinished() => Status == MatchStatus.Finished;
		public bool IsMatchWaiting() => Status == MatchStatus.Waiting;


		public GoalEvent AddGoal(int scoringTeamId, int goalScorerId, int period, int time, DateTime scoringMoment)
		{
			ValidateGoal(scoringTeamId, goalScorerId, period, time);

			GoalEvent goal = CreateGoal(scoringTeamId, goalScorerId, period, time);
			_goals.Add(goal);

			GetTeamById(scoringTeamId).AddGoal();

			if (Rules.MatchTimeRules.DoesGoalEndMatch(period))
			{
				goal.SetAsWinningGoal(true);
				IsOvertime = true;
			}
			return goal;
		}

		private void ValidateGoal(int scoringTeamId, int goalScorerId, int period, int time)
		{
			if (!IsMatchInProgress())
				throw new ArgumentException("Нельзя добавить гол, матчу, который сейчас не идёт");

			if (!IsTeamInMatch(scoringTeamId))
				throw new ArgumentException("Нельзя назначить забившей команду, которая не учавствует в матче");
			if (!IsPlayerInRoster(goalScorerId, scoringTeamId))
				throw new ArgumentException("Нельзя назначить автором гола игрока, которого нет в заявке за эту команду");

			ValidateGoalTiming(period, time);
		}

		private void ValidateGoalTiming(int period, int time)
		{
			if (!Rules.MatchTimeRules.IsValidPeriod(period))
				throw new ArgumentException("Проверьте значение периода, по правилам такого периода не существует");
			if (!Rules.MatchTimeRules.IsValidTimeInPeriod(period, time))
				throw new ArgumentException("Проверьте время, данный период не может иметь такое время.");
			if (Rules.MatchTimeRules.IsOvertimePeriod(period) && HomeTeam.Score != AwayTeam.Score)
				throw new ArgumentException("Нельзя добавить гол в овертайме, если у команд разный счёт");
		}

		private GoalEvent CreateGoal(int scoringTeamId, int goalScorerId, int period, int time)
			=> new(scoringTeamId, goalScorerId, period, time);


		private GoalEvent GetGoalEventById(int goalId)
		{
			GoalEvent goal = _goals.SingleOrDefault(goal => goal.Id == goalId);
			if (goal == null)
				throw new ArgumentException("Гол с данным Id не содержится в событиях этого матча");

			return goal;
		}

		public void FillGoalDetails(int goalId, int goalScorerId, int? firstAssistId, int? secondAssistId,
									GoalStrengthType strengthType, GoalNetType? netType)
		{
			GoalEvent goal = GetGoalEventById(goalId);
			if (!IsPlayerInRoster(goalScorerId, goal.ScoringTeamId))
				throw new ArgumentException("Игрок, которого вы пытаетесь установить автором гола, не заявлен за команду, которая забила гол");
			if (firstAssistId.HasValue && !IsPlayerInRoster(firstAssistId.Value, goal.ScoringTeamId))
				throw new ArgumentException("Игрок, которого вы пытаетесь установить как первого ассистента, не заявлен за команду, которая забила гол");
			if (secondAssistId.HasValue && !IsPlayerInRoster(secondAssistId.Value, goal.ScoringTeamId))
				throw new ArgumentException("Игрок, которого вы пытаетесь установить как второго ассистента, не заявлен за команду, которая забила гол");

			goal.SetScorer(goalScorerId);
			goal.SetAssists(firstAssistId, secondAssistId);
			goal.SetNetType(netType);
			goal.SetStrengthType(strengthType);
		}

		public void AddPlayerToRoster(int playerId, int teamId)
		{
			ValidateTeamInMatch(teamId);

			if (IsPlayerInRoster(playerId))
				throw new ArgumentException("Нельзя добавить игрока дважды");
			if (!IsMatchWaiting())
				throw new ArgumentException("Нельзя добавить игрока, после начала матча");

			GetTeamById(teamId).AddToRoster(playerId);
		}
		public void AddPlayersToRoster(List<int> playerIds, int teamId)
		{
			ValidateTeamInMatch(teamId);
			foreach (int playerId in playerIds)
				AddPlayerToRoster(playerId, teamId);
		}
		public void SetPlayersToRoster(List<int> playerIds, int teamId)
		{
			ValidateTeamInMatch(teamId);

			GetTeamById(teamId).ClearRoster();

			AddPlayersToRoster(playerIds, teamId);
		}
		private void ValidateTeamInMatch(int teamId)
		{
			if (!IsTeamInMatch(teamId))
				throw new ArgumentException("Команда не учавствует в матче");
		}
		public void SetScheduleAt(DateTime scheduleAt) => ScheduledAt = scheduleAt;
	}
}
