using SportsStats.Application.Matches.DTOs.Responses;
using SportsStats.Application.Players;
using SportsStats.Application.Teams;
using SportsStats.Application.Teams.DTOs.Responses;
using SportsStats.Application.Tournaments;
using SportsStats.Domain.Matches;
using SportsStats.Domain.Players;
using SportsStats.Domain.Services;
using SportsStats.Domain.Shared;
using SportsStats.Domain.Statistics;
using SportsStats.Domain.Teams;
using SportsStats.Domain.Tournaments;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Matches
{
	public class MatchQueriesHandler(
		IMatchRepository matchRepository,
		TournamentApplicationService tournamentApplicationService,
		TeamApplicationService teamApplicationService,
		PlayerApplicationService playerApplicationService
		)
	{
		private readonly IMatchRepository _matchRepository = matchRepository;
		private readonly TournamentApplicationService _tournamentApplicationService = tournamentApplicationService;
		private readonly TeamApplicationService _teamApplicationService = teamApplicationService;
		private readonly PlayerApplicationService _playerApplicationService = playerApplicationService;


		public async Task<MatchDTO?> GetAsync(int matchId)
		{
			Match? match = await _matchRepository.GetAsync(matchId);
			if (match == null)
				return null;

			var homeTeam = await _teamApplicationService.GetAsync(match.HomeTeamId);
			var awayTeam = await _teamApplicationService.GetAsync(match.AwayTeamId);
			var tournament = await _tournamentApplicationService.GetAsync(match.TournamentId);
			var homeTeamRoster = await _playerApplicationService.GetAsync(match.HomeTeamRoster.ToList());
			var awayTeamRoster = await _playerApplicationService.GetAsync(match.AwayTeamRoster.ToList());
			return MatchMapper.ToDTO(match, homeTeam, awayTeam, homeTeamRoster, awayTeamRoster, tournament);
		}
		public async Task<List<MatchShortDTO>> GetAllAsync(int tournamentId, int? teamId = null)
		{
			List<Match> matches = await _matchRepository.GetAllAsync(tournamentId, teamId);
			List<TeamDTO> teams = await _teamApplicationService.GetAllAsync(tournamentId);
			return matches.Select(m => MatchMapper.ToDTO(m, teams.First(t => t.Id == m.HomeTeamId), teams.First(t => t.Id == m.AwayTeamId))).ToList();
		}

	}
}
