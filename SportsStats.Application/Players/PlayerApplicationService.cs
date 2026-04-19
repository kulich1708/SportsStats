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
			Player player = await GetPlayerOrThrowAsync(playerId);
			string teamName = (await GetTeamOrThrowAsync(player.TeamId)).Name;
			string position = player.Position.GetDescription();

			return player == null ? null : PlayerMapper.ToDTO(player, teamName, position);
		}
		public async Task<List<PlayerDTO>> GetAsync(List<int> playerIds)
		{
			var players = await _playerRepository.GetAsync(playerIds);
			return await GetDTOAsync(players);
		}
		public async Task<List<PlayerDTO>> GetAllAsync(int? teamId = null)
		{
			var players = await _playerRepository.GetAllAsync(teamId);
			return await GetDTOAsync(players);
		}
		private async Task<List<PlayerDTO>> GetDTOAsync(List<Player> players)
		{
			var teamIds = players.Select(p => p.TeamId).Distinct().ToList();
			var teamNames = (await _teamRepository.GetAsync(teamIds)).ToDictionary(t => t.Id, t => t.Name);

			return players.Select(p => PlayerMapper.ToDTO(p, teamNames.GetValueOrDefault(p.TeamId), p.Position.GetDescription())).ToList();

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
