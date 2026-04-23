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
		public async Task<List<Team>> GetAsync(List<int> teamIds)
		{
			return await _context.Teams.Where(team => teamIds.Contains(team.Id)).ToListAsync();
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
		public async Task AddAsync(Team team)
		{
			await _context.Teams.AddAsync(team);
		}
		public async Task<List<Team>> GetAllAsync(int page, int pageSize, string? search = null)
		{
			return await _context.Teams
				.Where(t => search == null ? true : t.Name.ToLower().Contains(search.ToLower()))
				.OrderBy(t => t.Name)
				.ThenBy(t => t.City)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();
		}
		public async Task<List<Team>> GetByTournamentAsync(int tournamentId, string? search = null)
		{
			var tournament = await _context.Tournaments.FirstOrDefaultAsync(t => t.Id == tournamentId);
			return tournament == null ? [] :
				await _context.Teams
							  .Where(t => tournament.TeamsId.Contains(t.Id))
							  .Where(t => search == null ? true : t.Name.ToLower().Contains(search.ToLower()))
							  .ToListAsync();
		}
	}
}
