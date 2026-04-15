using SportsStats.Application.Statistics.DTOs.Responses;
using SportsStats.Domain.Statistics;
using SportsStats.Domain.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Statistics
{
	public class TeamStatsApplicationService(ITeamStatsRepository teamStatsRepository)
	{
		private readonly ITeamStatsRepository _teamStatsRepository = teamStatsRepository;
		public async Task<List<TeamStatsDTO>> GetByTeamAsync(int teamId)
		{
			List<TeamStats> stats = await _teamStatsRepository.GetByTeamAsync(teamId);
			return stats.Select(TeamStatsMapper.ToDTO).ToList();
		}

		public async Task<List<TeamStatsDTO>> GetByTournamentAsync(int tournamentId)
		{
			List<TeamStats> stats = await _teamStatsRepository.GetByTournamentAsync(tournamentId);
			return stats.Select(TeamStatsMapper.ToDTO).ToList();
		}
	}
}
