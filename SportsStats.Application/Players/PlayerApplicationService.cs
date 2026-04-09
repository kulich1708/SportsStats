using SportsStats.Application.Players.DTOs.Responses;
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

		public async Task<int> CreateAsync(string name, string surname, PositionType position)
		{
			Player player = new(name, surname, position);

			await _playerRepository.AddAsync(player);
			await _playerRepository.SaveChangesAsync();

			return player.Id;
		}
		public async Task ChangeTeamAsync(int playerId, int teamId)
		{
			Player player = await GetPlayerOrThrowAsync(playerId);
			await GetTeamOrThrowAsync(teamId);

			player.ChangeTeam(teamId);

			await _playerRepository.SaveChangesAsync();
		}
		public async Task<PlayerDTO?> GetAsync(int playerId)
		{
			Player player = await _playerRepository.GetAsync(playerId);
			return player == null ? null : PlayerMapper.ToDTO(player);
		}
		public async Task<List<PlayerDTO>> GetAllAsync(int? teamId = null)
		{
			return (await _playerRepository.GetAllAsync(teamId)).Select(PlayerMapper.ToDTO).ToList();
		}

		private async Task<Player> GetPlayerOrThrowAsync(int playerId)
		{
			return await _playerRepository.GetAsync(playerId)
				?? throw new ArgumentException("Игрок с таким Id не найден");
		}
		private async Task<Team> GetTeamOrThrowAsync(int teamId)
		{
			return await _teamRepository.GetAsync(teamId)
				?? throw new ArgumentException("Команда с таким id не найдена");
		}

	}
}
