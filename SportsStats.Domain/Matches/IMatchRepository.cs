using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Matches
{
	public interface IMatchRepository
	{
		public Task<Match?> GetAsync(int id);
		public Task SaveChangesAsync();
		// Для добавления НОВОЙ сущности
		public Task AddAsync(Match match);
	}
}
