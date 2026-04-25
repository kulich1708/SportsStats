using SportsStats.Domain.Matches;
using SportsStats.Domain.Matches.Goals;
using SportsStats.Domain.Players;
using SportsStats.Domain.Tournaments;
using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using SportsStats.Domain.Services;
using SportsStats.Application.Matches.DTOs.Responses;
using SportsStats.Domain.Teams;
using SportsStats.Domain.Statistics;

namespace SportsStats.Application.Matches
{
	public class MatchGoalService(
		IMatchRepository matchRepository,
		ITimeProvider timeProvider,
		MatchFinishService matchFinishService) : MatchUseCaseBase(matchRepository)
	{
		private readonly IMatchRepository _matchRepository = matchRepository;
		private readonly ITimeProvider _timeProvider = timeProvider;
		private readonly MatchFinishService _matchFinishService = matchFinishService;


		public async Task<int> AddGoalAsync(int matchId, int scoringTeamId, int goalScorerId, int period, int time)
		{
			Match match = await GetMatchOrThrowAsync(matchId);

			GoalEvent goal = match.AddGoal(scoringTeamId, goalScorerId, period, time, _timeProvider.GetCurrentTime());

			if (goal.IsWinning)
				await _matchFinishService.FinishAsync(matchId);
			else
				await _matchRepository.SaveChangesAsync();

			return goal.Id;
		}
		public async Task FillGoalDetailsAsync(int matchId, int goalId, int scorerId, int? firstAssistId, int? secondAssistId,
									GoalStrengthType strengthType, GoalNetType? netType = null)
		{
			Match match = await GetMatchOrThrowAsync(matchId);

			match.FillGoalDetails(goalId, scorerId, firstAssistId, secondAssistId, strengthType, netType);

			await _matchRepository.SaveChangesAsync();
		}
	}
}
