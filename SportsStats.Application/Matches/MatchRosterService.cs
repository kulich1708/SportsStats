using SportsStats.Domain.Common;
using SportsStats.Domain.Matches;
using SportsStats.Domain.Players;
using SportsStats.Domain.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Matches
{
	public class MatchRosterService(
		IMatchRepository matchRepository,
		IPlayerRepository playerRepository,
		IMatchService matchService,
		IUnitOfWork unitOfWork)
	{
		private readonly IMatchRepository _matchRepository = matchRepository;
		private readonly IPlayerRepository _playerRepository = playerRepository;
		private readonly IMatchService _matchService = matchService;
		private readonly IUnitOfWork _unitOfWork = unitOfWork;


		public async Task SetPlayersToRosterAsync(int matchId, List<int> playerIds, int teamId)
		{
			Match match = await _matchRepository.GetByIdAsync(matchId);

			List<Player> players = await _playerRepository.GetByIdAsync(playerIds);

			_matchService.SetRoster(match, players, teamId);

			await _unitOfWork.SaveChangesAsync();
		}
	}
}
