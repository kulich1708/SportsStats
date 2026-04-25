using SportsStats.Domain.Matches;
using SportsStats.Domain.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Matches
{
	public class MatchRosterService(
		IMatchRepository matchRepository,
		IPlayerRepository playerRepository) : MatchUseCaseBase(matchRepository)
	{
		private readonly IMatchRepository _matchRepository = matchRepository;
		private readonly IPlayerRepository _playerRepository = playerRepository;

		public async Task SetPlayersToRosterAsync(int matchId, List<int> playerIds, int teamId)
		{
			Match match = await GetMatchOrThrowAsync(matchId);

			List<Player> players = await _playerRepository.GetAsync(playerIds);

			foreach (var player in players)
				if (player.TeamId != teamId)
					throw new ArgumentException("Игрок не состоит в данной команде");

			match.SetPlayersToRoster(playerIds, teamId);

			await _matchRepository.SaveChangesAsync();
		}
	}
}
