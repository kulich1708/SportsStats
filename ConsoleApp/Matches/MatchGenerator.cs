using SportsStats.Application.Matches;
using SportsStats.Application.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp.Matches
{
	public class MatchGenerator(
		MatchGoalService matchGoalService,
		MatchLifecycleService matchLifecycleService,
		MatchFinishService matchFinishService,
		MatchRosterService matchRosterService,
		MatchQueriesHandler matchQueriesHandler,
		PlayerApplicationService playerApplicationService,
		GoalsGenerator goalsGenerator)
	{
		private readonly MatchGoalService _matchGoalService = matchGoalService;
		private readonly MatchLifecycleService _matchLifecycleService = matchLifecycleService;
		private readonly MatchFinishService _matchFinishService = matchFinishService;
		private readonly MatchRosterService _matchRosterService = matchRosterService;
		private readonly MatchQueriesHandler _matchQueriesHandler = matchQueriesHandler;
		private readonly PlayerApplicationService _playerApplicationService = playerApplicationService;
		private readonly GoalsGenerator _goalsGenerator = goalsGenerator;

		public async Task<int> GenerateMatchAsync(int homeTeamId, int awayTeamId, int tournamentId, DateTime scheduleAt)
		{
			int matchId = await _matchLifecycleService.CreateAsync(tournamentId, homeTeamId, awayTeamId, scheduleAt);

			await GenerateMatchRosterAsync(matchId, homeTeamId, awayTeamId);
			await _matchLifecycleService.StartAsync(matchId);
			await _goalsGenerator.GenerateGoalsAsync(matchId);
			if (!await _matchQueriesHandler.IsFinished(matchId))
				await _matchFinishService.FinishAsync(matchId);

			return matchId;
		}
		public async Task GenerateMatchRosterAsync(int matchId, int homeTeamId, int awayTeamId)
		{
			var homeTeamPlayers = await _playerApplicationService.GetByteamAsync(homeTeamId);
			var awayTeamPlayers = await _playerApplicationService.GetByteamAsync(awayTeamId);

			var homeTeamPlayerIds = homeTeamPlayers.Select(p => p.Id).ToList();
			var awayTeamPlayerIds = awayTeamPlayers.Select(p => p.Id).ToList();

			await _matchRosterService.SetPlayersToRosterAsync(matchId, homeTeamPlayerIds, homeTeamId);
			await _matchRosterService.SetPlayersToRosterAsync(matchId, awayTeamPlayerIds, awayTeamId);
		}
	}
}
