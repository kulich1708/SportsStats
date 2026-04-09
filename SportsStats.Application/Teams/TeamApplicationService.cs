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

		public async Task<Team> CreateAsync(string name)
		{
			Team team = new(name);

			await _teamRepository.AddAsync(team);
			await _teamRepository.SaveChangesAsync();

			return team;
		}

	}
}
