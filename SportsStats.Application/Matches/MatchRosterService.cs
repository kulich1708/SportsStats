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
		public async Task AddPlayerToRosterAsync(int matchId, int playerId, int teamId)
		{
			Match match = await GetMatchOrThrowAsync(matchId);
			Player player = await _playerRepository.GetAsync(playerId)
				?? throw new ArgumentException("Игрока с таким id не существует");

			if (player.TeamId != teamId)
				throw new ArgumentException("Игрок не состоит в данной команде");

			match.AddPlayerToRoster(playerId, teamId);

			await _matchRepository.SaveChangesAsync();
		}
	}
}
