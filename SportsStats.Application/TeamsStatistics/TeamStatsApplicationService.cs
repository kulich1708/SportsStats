using SportsStats.Application.Statistics.DTOs.Responses;
using SportsStats.Domain.Statistics;
using SportsStats.Domain.Teams;
using SportsStats.Domain.Tournaments;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Statistics
{
	public class TeamStatsApplicationService(
		ITeamStatsRepository teamStatsRepository,
		ITeamRepository teamRepository,
		ITournamentRepository tournamentRepository)
	{
		private readonly ITeamStatsRepository _teamStatsRepository = teamStatsRepository;
		private readonly ITeamRepository _teamRepository = teamRepository;
		private readonly ITournamentRepository _tournamentRepository = tournamentRepository;
		public async Task<List<TeamStatsDTO>> GetByTeamAsync(int teamId)
		{
			List<TeamStats> stats = await _teamStatsRepository.GetByTeamAsync(teamId);

			var teamName = (await _teamRepository.GetAsync(teamId))!.Name;
			var tournamentIds = stats.Select(s => s.TournamentId).Distinct().ToList();
			var tournamentNames = (await _tournamentRepository.GetAsync(tournamentIds))
								  .ToDictionary(t => t.Id, t => t.Name);

			return stats.Select(s => TeamStatsMapper.ToDTO(
				s, teamName,
				tournamentNames.GetValueOrDefault(s.TournamentId)!
			)).ToList();
		}

		public async Task<List<TeamStatsDTO>> GetByTournamentAsync(int tournamentId)
		{
			List<TeamStats> stats = await _teamStatsRepository.GetByTournamentAsync(tournamentId);

			var tournamentName = (await _tournamentRepository.GetAsync(tournamentId))!.Name;
			var teamNames = (await _teamRepository.GetAllAsync(tournamentId))
								  .ToDictionary(t => t.Id, t => t.Name);

			return stats.Select(s => TeamStatsMapper.ToDTO(
				s,
				teamNames.GetValueOrDefault(s.TeamId)!,
				tournamentName
			)).ToList();
		}
	}
}
