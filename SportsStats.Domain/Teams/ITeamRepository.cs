using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Teams
{
	public interface ITeamRepository
	{
		public Task<Team?> FindByIdAsync(int teamId);
		public Task<Team> GetByIdAsync(int teamId);
		public Task<List<Team>> GetByIdAsync(List<int> teamIds);
		public void Add(Team team);
		public Task<List<Team>> GetAllAsync(int page, int pageSize, string? search = null);
		public Task<List<Team>> GetByTournamentAsync(int tournamentId, string? search = null);

	}
}
