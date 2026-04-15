using Microsoft.EntityFrameworkCore;
using SportsStats.Domain.Statistics;
using SportsStats.Domain.Teams;
using SportsStats.Domain.Tournaments;
using SportsStats.Infrastructure.Persistence.DbContexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Infrastructure.Persistence.Repositories
{
	public class TeamStatsRepository(AppDbContext context) : ITeamStatsRepository
	{
		private readonly AppDbContext _context = context;
		public async Task AddAsync(TeamStats stats)
		{
			await _context.AddAsync(stats);
		}

		public async Task<TeamStats> GetAsync(int teamId, int tournamentId)
		{
			return await _context.TeamsStats.FirstAsync(stats => stats.TeamId == teamId && stats.TournamentId == tournamentId);
		}

		public async Task<List<TeamStats>> GetByTeamAsync(int teamId)
		{
			return await _context.TeamsStats.Where(stats => stats.TeamId == teamId).ToListAsync();
		}

		public async Task<List<TeamStats>> GetByTournamentAsync(int tournamentId)
		{
			return await _context.TeamsStats.Where(stats => stats.TournamentId == tournamentId).ToListAsync();
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}
