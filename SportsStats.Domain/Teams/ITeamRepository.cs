using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Teams
{
	public interface ITeamRepository
	{
		public Task<Team?> FindById(int teamId);
		public Task SaveChangesAsync();
		public Task AddAsync(Team team);
	}
}
