using SportsStats.Domain.Players;
using SportsStats.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using SportsStats.Domain.Teams;
using SportsStats.Domain.Shared;

namespace SportsStats.Infrastructure.Persistence.Repositories
{
	public class PlayerRepository(AppDbContext context)
		: BaseRepository<Player>(context), IPlayerRepository
	{

		private readonly AppDbContext _context = context;
		protected override ErrorCode NotFoundErrorCode => NotFoundError.Player;

		public override async Task<Player?> FindByIdAsync(int playerId)
		{
			return await _context.Players.FirstOrDefaultAsync(player => player.Id == playerId);
		}
		public async Task<List<Player>> GetByTeamAsync(int teamId)
		{
			Team? team = await _context.Teams.FirstOrDefaultAsync(t => t.Id == teamId);
			return team == null ? [] : await _context.Players.Where(p => p.TeamId == teamId).ToListAsync();
		}
		public async Task<List<Player>> GetAllAsync(int page, int pageSize, string? search = null)
		{
			return await _context.Players
				.Where(p => search == null ? true : p.Surname.ToLower().Contains(search.ToLower()))
				.OrderBy(p => p.Surname)
				.ThenBy(p => p.Name)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();
		}
		public async Task<List<Player>> GetByIdAsync(List<int> playersId)
		{
			return await _context.Players.Where(p => playersId.Contains(p.Id)).ToListAsync();
		}
	}
}
