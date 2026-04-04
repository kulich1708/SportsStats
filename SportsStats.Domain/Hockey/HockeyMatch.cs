using SportsStats.Domain.Matches;
using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Hockey
{
	internal class HockeyMatch : MatchBase<HockeyGoalEvent>
	{
		public HockeyMatch(int homeTeamId, int awayTeamId, int tournamentId,
						 IExistenceChecker teamExistenceChecker,
						 IExistenceChecker tournamentExistenceChecker,
						 IPlayerDataProvider playerDataProvider,
						 ITournamentDataProvider tournamentDataProvider)
			: base(homeTeamId, awayTeamId, tournamentId, teamExistenceChecker, tournamentExistenceChecker, playerDataProvider, tournamentDataProvider) { }
		protected override HockeyGoalEvent CreateGoal(int scoringTeamId, int goalScorerId, int period, int time)
		{
			return new HockeyGoalEvent(Id, scoringTeamId, goalScorerId, period, time);
		}
		// TODO разделить этот метод (для возможности редактирования гола в любой момент), на много маленьких, но будет проблема, что матч оброс множеством методов для гола.
		// Решение: вынести управление голами в GoalRedactor и передавать его в матч.
		// Со временем отказаться от наследования матчей и собирать матч как конструктор из разных блоков
		// Например из PenaltyRedactor, GoalRedactor, PeriodRedactor и тд.
		public void FillGoalDetails(int goalId, int? firstAssistId, int? secondAssistId,
									HockeyGoalStrengthType strengthType, HockeyGoalNetType? netType)
		{
			HockeyGoalEvent goal = GetGoalById(goalId);
			if (firstAssistId.HasValue && !IsPlayerOnMatchRoster(firstAssistId.Value))
				throw new ArgumentException("Этот игрок не заявлен за команду, которая забила гол");
			if (secondAssistId.HasValue && !IsPlayerOnMatchRoster(secondAssistId.Value))
				throw new ArgumentException("Этот игрок не заявлен за команду, которая забила гол");

			goal.SetAssists(firstAssistId, secondAssistId);
			goal.SetNetType(netType);
			goal.SetStrengthType(strengthType);
		}
	}
}
