using SportsStats.Domain.Players;
using SportsStats.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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
	}
}
