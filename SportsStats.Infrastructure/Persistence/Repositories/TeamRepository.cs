using Microsoft.EntityFrameworkCore;
using SportsStats.Domain.Teams;
using SportsStats.Infrastructure.Persistence.DbContexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Infrastructure.Persistence.Repositories
{
	public class TeamRepository(AppDbContext context) : ITeamRepository
	{
		private readonly AppDbContext _context = context;

		public async Task<Team?> GetAsync(int teamId)
		{
			return await _context.Teams.FirstOrDefaultAsync(team => team.Id == teamId);
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
		public async Task AddAsync(Team team)
		{
			await _context.Teams.AddAsync(team);
		}
	}
}
