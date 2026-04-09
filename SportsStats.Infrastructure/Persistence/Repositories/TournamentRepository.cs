using Microsoft.EntityFrameworkCore;
using SportsStats.Domain.Tournaments;
using SportsStats.Infrastructure.Persistence.DbContexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Infrastructure.Persistence.Repositories
{
	public class TournamentRepository : ITournamentRepository
	{
		private readonly AppDbContext _context;

		public TournamentRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<Tournament?> FindById(int tournamentId)
		{
			return await _context.Tournaments.FirstOrDefaultAsync(t => t.Id == tournamentId);
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
		public async Task AddAsync(Tournament tournament)
		{
			await _context.Tournaments.AddAsync(tournament);
		}
	}
}
