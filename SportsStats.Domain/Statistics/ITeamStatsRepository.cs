using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Statistics
{
	public interface ITeamStatsRepository
	{
		public Task<TeamStats> GetAsync(int teamId, int tournamentId);
		public Task<List<TeamStats>> GetByTeamAsync(int teamId);
		public Task<List<TeamStats>> GetByTournamentAsync(int tournamentId);
		public Task SaveChangesAsync();
		public Task AddAsync(TeamStats stats);
	}
}
