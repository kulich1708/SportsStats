using SportsStats.Domain.Shared;
using SportsStats.Domain.Teams;
using SportsStats.Domain.Tournaments;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SportsStats.Application.Teams
{
	public class TeamApplicationService
	{
		private ITeamRepository _teamRepository;
		public TeamApplicationService(ITeamRepository teamRepository)
		{
			_teamRepository = teamRepository;
		}

		public Team Create(string name)
		{
			Team team = new Team(name);
			return _teamRepository.Save(team);
		}

	}
}
