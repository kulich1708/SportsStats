using SportsStats.Domain.Players;
using SportsStats.Domain.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Players
{
	public class PlayerApplicationService(IPlayerRepository playerRepository, ITeamRepository teamRepository)
	{
		private readonly IPlayerRepository _playerRepository = playerRepository;
		private readonly ITeamRepository _teamRepository = teamRepository;

		public async Task<Player> Create(string name, string surname, PositionType position)
		{
			Player player = new(name, surname, position);
			return await _playerRepository.Save(player);
		}
		public async Task ChangeTeam(int playerId, int teamId)
		{
			Player player = await GetPlayerOrThrow(playerId);
			await GetTeamOrThrow(teamId);

			player.ChangeTeam(teamId);

			await _playerRepository.Save(player);
		}

		private async Task<Player> GetPlayerOrThrow(int playerId)
		{
			return await _playerRepository.FindById(playerId)
				?? throw new ArgumentException("Игрок с таким Id не найден");
		}
		private async Task<Team> GetTeamOrThrow(int teamId)
		{
			return await _teamRepository.FindById(teamId)
				?? throw new ArgumentException("Команда с таким id не найдена");
		}
	}
}
