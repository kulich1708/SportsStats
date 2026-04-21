using SportsStats.Application.Teams.DTOs.Responses;
using SportsStats.Domain.Shared;
using SportsStats.Domain.Teams;
using SportsStats.Domain.Tournaments;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SportsStats.Application.Teams
{
	public class TeamApplicationService(ITeamRepository teamRepository)
	{
		private readonly ITeamRepository _teamRepository = teamRepository;

		public async Task<int> CreateAsync(string name)
		{
			Team team = new(name);

			await _teamRepository.AddAsync(team);
			await _teamRepository.SaveChangesAsync();

			return team.Id;
		}
		public async Task<TeamDTO?> GetAsync(int id)
		{
			Team? team = await _teamRepository.GetAsync(id);
			return team == null ? null : TeamMapper.ToDTO(team);
		}
		public async Task<List<TeamDTO>> GetAllAsync(int page, int pageSize, string? search = null)
		{
			var teams = await _teamRepository.GetAllAsync(page, pageSize, search);
			return teams.Select(TeamMapper.ToDTO).ToList();
		}
		public async Task<List<TeamDTO>> GetByTournamentAsync(int tournamentId)
		{
			var teams = await _teamRepository.GetByTournamentAsync(tournamentId);
			return teams.Select(TeamMapper.ToDTO).ToList();
		}
		public async Task ChangeGeneralInfo(int id, string name, string? city, byte[]? photo, string? photoMime)
		{
			var team = await _teamRepository.GetAsync(id)
				?? throw new ArgumentException("Команда с таким id не найдена");
			team.SetName(name);
			team.SetCity(city);
			team.SetPhoto(photo, photoMime);

			await _teamRepository.SaveChangesAsync();
		}
	}
}
