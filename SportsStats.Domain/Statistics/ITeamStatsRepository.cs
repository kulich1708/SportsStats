using SportsStats.Domain.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Statistics
{
	public interface ITeamStatsRepository
	{
		public Task<TeamStats> GetAsync(int teamId, int tournamentId);
		public Task<List<TeamStats>> GetByTeamAsync(int teamId);
		public void Add(TeamStats teamStats);
		public Task<List<TeamStats>> GetByTournamentAsync(int tournamentId);
	}
}
