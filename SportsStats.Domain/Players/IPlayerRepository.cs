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
		public Task<List<Player>> GetByTeamAsync(int teamId);
		public Task<List<Player>> GetAllAsync(int page, int pageSize, string? search = null);
		public Task<List<Player>> GetAsync(List<int> playersId);
	}
}
