using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments
{
	public interface ITournamentRepository
	{
		public Task<Tournament?> GetAsync(int tournamentId);
		public Task SaveChangesAsync();
		public Task AddAsync(Tournament tournament);
		public Task<List<Tournament>> GetAllAsync(bool onlyStarted);
	}
}
