using SportsStats.Domain.Matches;
using SportsStats.Domain.Teams;
using SportsStats.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Infrastructure.Persistence.Repositories
{
	public class MatchRepository(AppDbContext context) : IMatchRepository
	{
		private readonly AppDbContext _context = context;

		public async Task<Match?> GetAsync(int matchId)
		{
			return await _context.Matches.Include(match => match.Goals).FirstOrDefaultAsync(match => match.Id == matchId);
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
		public async Task AddAsync(Match match)
		{
			await _context.Matches.AddAsync(match);
		}
	}
}
