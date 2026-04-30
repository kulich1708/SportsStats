using SportsStats.Application.Matches;
using SportsStats.Application.Players.DTOs.Responses;
using SportsStats.Application.Teams.DTOs.Responses;
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
				int goalsCount = _random.Next(1, 4);
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
				difference += scoringTeamIndex == 0 ? 1 : -1;
				int goalScorerIndex = _random.Next(0, rosters[scoringTeamIndex].Count);
				int goalScorerId = rosters[scoringTeamIndex][goalScorerIndex].Id;
				int time = _random.Next(startTime, endTime);

				await _matchGoalService.AddGoalAsync(matchId, teams[scoringTeamIndex].Id, goalScorerId, period, time);
				return time;
			}
		}
	}
}
