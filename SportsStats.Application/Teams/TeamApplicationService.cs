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
		public async Task<List<TeamDTO>> GetAllAsync(int? tournamentId = null)
		{
			var teams = await _teamRepository.GetAllAsync(tournamentId);
			return teams.Select(TeamMapper.ToDTO).ToList();
		}

	}
}
