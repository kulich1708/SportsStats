using SportsStats.Application.Players.DTOs.Requests;
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
			string? teamName = player.TeamId.HasValue ? (await GetTeamOrThrowAsync(player.TeamId.Value)).Name : null;
			string position = player.Position.GetDescription();

			return player == null ? null : PlayerMapper.ToDTO(player, teamName, position);
		}
		public async Task<List<PlayerDTO>> GetAsync(List<int> playerIds)
		{
			var players = await _playerRepository.GetAsync(playerIds);
			return await GetDTOAsync(players);
		}
		public async Task<List<PlayerDTO>> GetByteamAsync(int teamId)
		{
			var players = await _playerRepository.GetByTeamAsync(teamId);
			return await GetDTOAsync(players);
		}
		public async Task<List<PlayerDTO>> GetAllAsync(int page, int pageSize, string? search = null)
		{
			var players = await _playerRepository.GetAllAsync(page, pageSize, search);
			return await GetDTOAsync(players);
		}
		private async Task<List<PlayerDTO>> GetDTOAsync(List<Player> players)
		{
			var teamIds = players.Where(p => p.TeamId.HasValue).Select(p => p.TeamId!.Value).Distinct().ToList();
			var teamNames = (await _teamRepository.GetAsync(teamIds)).ToDictionary(t => t.Id, t => t.Name);

			return players
				.Select(p => PlayerMapper.ToDTO(p, p.TeamId.HasValue ? teamNames.GetValueOrDefault(p.TeamId.Value) : null, p.Position.GetDescription()))
				.ToList();
		}
		public async Task ChangeGeneralInfoAsync(int id, PlayerGeneralInfoDTO dto)
		{
			var player = await GetPlayerOrThrowAsync(id);
			player.SetNameAndSurname(dto.Name, dto.Surname);
			player.SetPosition(dto.Position);

			if (dto.Birthday.HasValue)
				player.SetBirthday(dto.Birthday.Value);

			if (dto.Number.HasValue)
				player.SetNumber(dto.Number.Value);

			if (dto.Photo != null)
				player.SetPhoto(dto.Photo, dto.PhotoMime);

			if (dto.Citizenship != null && string.IsNullOrWhiteSpace(dto.Citizenship.Name))
				player.SetCitizenship(dto.Citizenship.Name, dto.Citizenship.Photo, dto.Citizenship.PhotoMime);

			if (dto.TeamId.HasValue)
				player.ChangeTeam(dto.TeamId.Value);
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
