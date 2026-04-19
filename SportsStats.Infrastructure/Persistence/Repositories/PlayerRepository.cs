using SportsStats.Domain.Players;
using SportsStats.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using SportsStats.Domain.Teams;

namespace SportsStats.Infrastructure.Persistence.Repositories
{
	public class PlayerRepository(AppDbContext context) : IPlayerRepository
	{

		private readonly AppDbContext _context = context;

		public async Task<Player?> GetAsync(int playerId)
		{
			return await _context.Players.FirstOrDefaultAsync(player => player.Id == playerId);
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
		public async Task AddAsync(Player player)
		{
			await _context.Players.AddAsync(player);
		}
		public async Task<List<Player>> GetAllAsync(int? teamId = null)
		{
			if (teamId == null)
				return await _context.Players.ToListAsync();

			Team team = await _context.Teams.FirstOrDefaultAsync(t => t.Id == teamId);
			return team == null ? [] : await _context.Players.Where(p => p.TeamId == teamId).ToListAsync();
		}
		public async Task<List<Player>> GetAsync(List<int> playersId)
		{
			return await _context.Players.Where(p => playersId.Contains(p.Id)).ToListAsync();
		}
	}
}
