using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Matches
{
	public interface IMatchRepository
	{
		public Task<Match?> GetAsync(int id);
		public Task SaveChangesAsync();
		public Task AddAsync(Match match);

		public Task<List<Match>> GetByDateAsync(DateOnly date, int? tournamentId = null);
		public Task<List<Match>> GetByDateAsync(DateOnly date, List<int>? tournamentIds = null);
		public Task<List<Match>> GetFinishedByTeamAsync(int teamId, int page, int pageSize);
		public Task<List<Match>> GetScheduleByTeamAsync(int teamId, int page, int pageSize);
		public Task<List<Match>> GetFinishedByTournamentAsync(int tournamentId, int page, int pageSize);
		public Task<List<Match>> GetScheduleByTournamentAsync(int tournamentId, int page, int pageSize);


		public Task<int> GetUnfinishedMatchCountAsync(int tournamentId);
		public Task<DateTime> GetLastMatchFinishedAtAsync(int tournamentId);
	}
}
