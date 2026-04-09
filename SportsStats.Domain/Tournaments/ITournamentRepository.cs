using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments
{
	public interface ITournamentRepository
	{
		public Task<Tournament?> FindById(int tournamentId);
		public Task SaveChangesAsync();
		public Task AddAsync(Tournament tournament);
	}
}
