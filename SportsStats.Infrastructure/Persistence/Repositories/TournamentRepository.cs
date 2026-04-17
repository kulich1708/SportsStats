using Microsoft.EntityFrameworkCore;
using SportsStats.Domain.Tournaments;
using SportsStats.Infrastructure.Persistence.DbContexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Infrastructure.Persistence.Repositories
{
	public class TournamentRepository(AppDbContext context) : ITournamentRepository
	{
		private readonly AppDbContext _context = context;

		public async Task<Tournament?> GetAsync(int tournamentId)
		{
			return await _context.Tournaments.FirstOrDefaultAsync(t => t.Id == tournamentId);
		}
		public async Task<List<Tournament>> GetAsync(List<int> tournamentIds)
		{
			return await _context.Tournaments.Where(t => tournamentIds.Contains(t.Id)).ToListAsync();
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
		public async Task AddAsync(Tournament tournament)
		{
			await _context.Tournaments.AddAsync(tournament);
		}

		/// <summary>
		/// NOTE: Логика фильтрации должна совпадать с методом Tournament.IsStarted()
		/// </summary>
		public async Task<List<Tournament>> GetAllAsync(bool onlyStarted)
		{
			if (onlyStarted)
				return await _context.Tournaments.Where(t => t.Status == TournamentStatus.InProgress).ToListAsync();
			return await _context.Tournaments.ToListAsync();
		}

		public async Task<List<Tournament>> GetActiveByDateAsync(DateOnly date)
		{
			var startOfDay = DateTime.SpecifyKind(date.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
			var endOfDay = DateTime.SpecifyKind(date.ToDateTime(TimeOnly.MaxValue), DateTimeKind.Utc);
			return await _context.Tournaments
				.Where(t => t.StartedAt.HasValue && t.StartedAt.Value <= endOfDay && (!t.FinishedAt.HasValue || startOfDay <= t.FinishedAt.Value))
				.ToListAsync();
		}
	}
}
