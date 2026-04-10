using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Matches
{
	public interface IMatchRepository
	{
		public Task<Match?> GetAsync(int id);
		public Task SaveChangesAsync();
		public Task AddAsync(Match match);
		public Task<List<Match>> GetAllAsync(int tournamentId, int? teamId = null);
	}
}
