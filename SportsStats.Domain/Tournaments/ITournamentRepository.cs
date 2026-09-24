using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments
{
	public interface ITournamentRepository
	{
		public Task<Tournament?> FindByIdAsync(int tournamentId);
		public Task<Tournament> GetByIdAsync(int tournamentId);
		public Task<List<Tournament>> GetByIdAsync(List<int> tournamentIds);
		public void Add(Tournament tournament);
		public Task<List<Tournament>> GetAllAsync(int page, int pageSize, string? search = null);
		public Task<List<Tournament>> GetActiveByDateAsync(DateOnly date);
	}
}
