using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Teams
{
	public interface ITeamRepository
	{
		public Task<Team?> GetAsync(int teamId);
		public Task<List<Team>> GetAsync(List<int> teamIds);
		public Task SaveChangesAsync();
		public Task AddAsync(Team team);
		public Task<List<Team>> GetAllAsync(int page, int pageSize);
		public Task<List<Team>> GetByTournamentAsync(int tournamentId);

	}
}
