using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Players
{
	public interface IPlayerRepository
	{
		public Task<Player?> GetAsync(int playerId);
		public Task SaveChangesAsync();
		public Task AddAsync(Player player);
	}
}
