using SportsStats.Application.Matches;
using SportsStats.Application.Players.DTOs.Responses;
using SportsStats.Application.Teams.DTOs.Responses;
using SportsStats.Domain.Matches.Goals;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp.Matches
{
	public class GoalsGenerator(
		MatchQueriesHandler matchQueriesHandler,
		MatchGoalService matchGoalService)
	{
		private readonly MatchQueriesHandler _matchQueriesHandle = matchQueriesHandler;
		private readonly MatchGoalService _matchGoalService = matchGoalService;
		private readonly Random _random = new();
		public async Task GenerateGoalsAsync(int matchId)
		{
			var match = await _matchQueriesHandle.GetAsync(matchId);

			List<TeamDTO> teams = [match.HomeTeam, match.AwayTeam];
			List<List<PlayerDTO>> rosters = [match.HomeTeamRoster, match.AwayTeamRoster];

			int difference = 0;

			for (int period = 1; period < match.Rules.MatchTimeRules.PeriodsCount + 1; period++)
			{
				int goalsCount = _random.Next(1, 3);
				int lastTime = 0;
				for (int i = 0; i <= goalsCount; i++)
					lastTime = await GenerateGoalAsync(period, lastTime, match.Rules.MatchTimeRules.PeriodDurationSeconds);
			}

			if (difference == 0 && !match.Rules.MatchTimeRules.IsDrawPossible && match.Rules.MatchTimeRules.HasOvertime)
			{
				await GenerateGoalAsync(match.Rules.MatchTimeRules.PeriodsCount + 1, 0, match.Rules.MatchTimeRules.OvertimeRules!.OvertimeDurationSeconds ?? 2400);
			}


			async Task<int> GenerateGoalAsync(int period, int startTime, int endTime)
			{
				int scoringTeamIndex = _random.Next(0, 2);
				int rosterCount = rosters[scoringTeamIndex].Count;
				difference += scoringTeamIndex == 0 ? 1 : -1;

				List<int> forbiddenPlayerIndexes = new();
				int goalScorerIndex = GeneratePlayerIndex(forbiddenPlayerIndexes, rosterCount);
				int goalScorerId = rosters[scoringTeamIndex][goalScorerIndex].Id;
				int? firstAssistIndex = null;
				int? secondAssistIndex = null;
				int? firstAssistId = null;
				int? secondAssistId = null;

				bool isFirstAssist = _random.Next(0, 5) != 0;
				if (isFirstAssist)
				{
					firstAssistIndex = GeneratePlayerIndex(forbiddenPlayerIndexes, rosterCount);
					firstAssistId = rosters[scoringTeamIndex][firstAssistIndex.Value].Id;

					bool isSecondAssist = _random.Next(0, 5) != 0;

					if (isSecondAssist)
					{
						secondAssistIndex = GeneratePlayerIndex(forbiddenPlayerIndexes, rosterCount);
						secondAssistId = rosters[scoringTeamIndex][secondAssistIndex.Value].Id;
					}
				}

				int time = _random.Next(startTime, endTime);
				int strengthInt = _random.Next(0, 20);
				GoalStrengthType strengthType = GoalStrengthType.EvenStrength;
				if (strengthInt == 0)
					strengthType = GoalStrengthType.Shorthanded;
				else if (strengthInt < 5)
					strengthType = GoalStrengthType.PowerPlay;


				int goalId = await _matchGoalService.AddGoalAsync(matchId, teams[scoringTeamIndex].Id, goalScorerId, time);
				await _matchGoalService.FillGoalDetailsAsync(matchId, goalId, goalScorerId, firstAssistId, secondAssistId, strengthType);
				return time;
			}
		}
		public int GeneratePlayerIndex(List<int> forbidden, int maxValue)
		{
			int index = _random.Next(0, maxValue);
			while (forbidden.Contains(index))
				index = _random.Next(0, maxValue);
			forbidden.Add(index);
			return index;
		}
	}
}
