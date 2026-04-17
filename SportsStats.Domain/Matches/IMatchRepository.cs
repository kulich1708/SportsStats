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

		public Task<List<Match>> GetByDate(int tournamentId, DateOnly date);

		public Task<int> GetUnfinishedMatchCountAsync(int tournamentId);
		public Task<DateTime> GetLastMatchFinishedAtAsync(int tournamentId);
	}
}
