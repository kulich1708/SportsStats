using SportsStats.Domain.Common;
using SportsStats.Domain.Matches;
using SportsStats.Domain.Matches.Goals;
using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Matches
{
	public class MatchGoalService(
		IMatchRepository matchRepository,
		ITimeProvider timeProvider,
		IUnitOfWork unitOfWork)
	{
		private readonly IMatchRepository _matchRepository = matchRepository;
		private readonly ITimeProvider _timeProvider = timeProvider;
		private readonly IUnitOfWork _unitOfWork = unitOfWork;

		public async Task<int> AddGoalAsync(int matchId, int scoringTeamId, int goalScorerId, int time)
		{
			Match match = await _matchRepository.GetByIdAsync(matchId);

			GoalEvent goal = match.AddGoal(scoringTeamId, goalScorerId, time, _timeProvider.GetCurrentTime());

			await _unitOfWork.SaveChangesAsync();

			return goal.Id;
		}
		public async Task FillGoalDetailsAsync(int matchId, int goalId, int scorerId, int? firstAssistId, int? secondAssistId,
									GoalStrengthType strengthType, GoalNetType? netType = null)
		{
			Match match = await _matchRepository.GetByIdAsync(matchId);

			match.FillGoalDetails(goalId, scorerId, firstAssistId, secondAssistId, strengthType, netType);

			await _unitOfWork.SaveChangesAsync();
		}
	}
}
