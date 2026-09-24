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
			Player player = await _playerRepository.GetByIdAsync(playerId);
			await _teamRepository.GetByIdAsync(teamId);

			player.ChangeTeam(teamId);

			await _playerRepository.SaveChangesAsync();
		}
		public async Task<PlayerDTO> GetByIdAsync(int playerId)
		{
			Player player = await _playerRepository.GetByIdAsync(playerId);
			string? teamName = player.TeamId.HasValue ? (await _teamRepository.GetByIdAsync(player.TeamId.Value)).Name : null;

			return PlayerMapper.ToDTO(player, teamName);
		}
		public async Task<List<PlayerDTO>> GetByIdAsync(List<int> playerIds)
		{
			var players = await _playerRepository.GetByIdAsync(playerIds);
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
			var teamNames = (await _teamRepository.GetByIdAsync(teamIds)).ToDictionary(t => t.Id, t => t.Name);

			return players
				.Select(p => PlayerMapper.ToDTO(p, p.TeamId.HasValue ? teamNames.GetValueOrDefault(p.TeamId.Value) : null))
				.ToList();
		}
		public async Task ChangeGeneralInfoAsync(int id, PlayerGeneralInfoDTO dto)
		{
			var player = await _playerRepository.GetByIdAsync(id);

			player.SetNameAndSurname(dto.Name, dto.Surname);
			player.SetPosition(dto.Position);
			player.SetBirthday(dto.Birthday);
			player.SetNumber(dto.Number);
			player.SetPhoto(dto.Photo, dto.PhotoMime);
			player.SetCitizenship(dto.Citizenship?.Name, dto.Citizenship?.Photo, dto.Citizenship?.PhotoMime);
			player.ChangeTeam(dto.TeamId);

			await _playerRepository.SaveChangesAsync();
		}
		public static IReadOnlyDictionary<PositionType, string> GetAllPlayerPositions() => PositionTypeText.PositionDescription;
	}
}
