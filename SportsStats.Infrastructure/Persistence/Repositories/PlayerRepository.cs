using SportsStats.Domain.Players;
using SportsStats.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Infrastructure.Persistence.Repositories
{
	public class PlayerRepository : IPlayerRepository
	{

		private readonly AppDbContext _context;

		public PlayerRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<Player?> FindById(int playerId)
		{
			return await _context.Players.FirstOrDefaultAsync(player => player.Id == playerId);
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
		public async Task AddAsync(Player player)
		{
			await _context.AddAsync(player);
		}
	}
}
