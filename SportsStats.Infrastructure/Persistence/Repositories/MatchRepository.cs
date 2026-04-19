using Microsoft.EntityFrameworkCore;
using SportsStats.Domain.Matches;
using SportsStats.Domain.Teams;
using SportsStats.Domain.Tournaments;
using SportsStats.Infrastructure.Persistence.DbContexts;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
		public async Task<List<Match>> GetAllAsync(int tournamentId, int? teamId = null)
		{
			var mathces = _context.Matches.Where(m => m.TournamentId == tournamentId);
			if (teamId == null)
				return await mathces.ToListAsync();

			return await mathces.Where(m => m.HomeTeamId == teamId || m.AwayTeamId == teamId).ToListAsync();
		}
		public async Task<List<Match>> GetByDateAsync(DateOnly date, int? tournamentId = null)
		{
			return await GetByDateAsync(date, tournamentId.HasValue ? [tournamentId.Value] : null);
		}
		public async Task<List<Match>> GetByDateAsync(DateOnly date, List<int>? tournamentIds = null)
		{
			var startOfDay = DateTime.SpecifyKind(date.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
			var endOfDay = DateTime.SpecifyKind(date.ToDateTime(TimeOnly.MaxValue), DateTimeKind.Utc);

			return await _context.Matches
				.Where(m => (tournamentIds == null || tournamentIds.Contains(m.TournamentId)) && (
					m.ScheduledAt >= startOfDay && m.ScheduledAt <= endOfDay ||
					(m.StartedAt.HasValue && m.StartedAt.Value >= startOfDay && m.StartedAt.Value <= endOfDay) ||
					(m.FinishedAt.HasValue && m.FinishedAt.Value >= startOfDay && m.FinishedAt.Value <= endOfDay)
				))
				.ToListAsync();
		}
		public async Task<int> GetUnfinishedMatchCountAsync(int tournamentId)
		{
			return (await _context.Matches
				.Where(m => m.TournamentId == tournamentId && m.Status != MatchStatus.Finished)
				.ToListAsync()).Count;
		}
		public async Task<DateTime> GetLastMatchFinishedAtAsync(int tournamentId)
		{
			var lastMatch = await _context.Matches
				.Where(m => m.TournamentId == tournamentId && m.FinishedAt != null)
				.OrderByDescending(m => m.FinishedAt)
				.FirstOrDefaultAsync();

			return lastMatch?.FinishedAt ?? DateTime.MinValue;
		}

		public async Task<List<Match>> GetFinishedByTeamAsync(int teamId, int page, int pageSize)
		{
			return await _context.Matches
				.Where(m => (m.HomeTeamId == teamId || m.AwayTeamId == teamId) && m.Status == MatchStatus.Finished)
				.OrderByDescending(m => m.FinishedAt)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();
		}
		public async Task<List<Match>> GetScheduleByTeamAsync(int teamId, int page, int pageSize)
		{
			return await _context.Matches
				.Where(m => (m.HomeTeamId == teamId || m.AwayTeamId == teamId) && m.Status != MatchStatus.Finished)
				.OrderBy(m => m.ScheduledAt)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();
		}

		public async Task<List<Match>> GetFinishedByTournamentAsync(int tournamentId, int page, int pageSize)
		{
			return await _context.Matches
				.Where(m => m.TournamentId == tournamentId && m.Status == MatchStatus.Finished)
				.OrderByDescending(m => m.FinishedAt)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();
		}
		public async Task<List<Match>> GetScheduleByTournamentAsync(int tournamentId, int page, int pageSize)
		{
			return await _context.Matches
				.Where(m => m.TournamentId == tournamentId && m.Status != MatchStatus.Finished)
				.OrderBy(m => m.ScheduledAt)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();
		}
	}
}
