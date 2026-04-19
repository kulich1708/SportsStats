using SportsStats.Application.Matches.DTOs.Responses;
using SportsStats.Application.Players;
using SportsStats.Application.Teams;
using SportsStats.Application.Teams.DTOs.Responses;
using SportsStats.Application.Tournaments;
using SportsStats.Domain.Matches;
using SportsStats.Domain.Players;
using SportsStats.Domain.Teams;
using SportsStats.Domain.Tournaments;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Matches
{
	public class MatchQueriesHandler(
		IMatchRepository matchRepository,
		ITeamRepository teamRepository,
		ITournamentRepository tournamentRepository,
		TeamApplicationService teamApplicationService,
		PlayerApplicationService playerApplicationService
		) : MatchUseCaseBase(matchRepository)
	{
		private readonly IMatchRepository _matchRepository = matchRepository;
		private readonly ITeamRepository _teamRepository = teamRepository;
		private readonly ITournamentRepository _tournamentRepository = tournamentRepository;
		private readonly TeamApplicationService _teamApplicationService = teamApplicationService;
		private readonly PlayerApplicationService _playerApplicationService = playerApplicationService;


		public async Task<MatchDTO?> GetAsync(int matchId)
		{
			Match? match = await _matchRepository.GetAsync(matchId);
			if (match == null)
				return null;

			var homeTeam = await _teamApplicationService.GetAsync(match.HomeTeamId);
			var awayTeam = await _teamApplicationService.GetAsync(match.AwayTeamId);
			var tournament = await _tournamentRepository.GetAsync(match.TournamentId);
			var homeTeamRoster = await _playerApplicationService.GetAsync(match.HomeTeamRoster.ToList());
			var awayTeamRoster = await _playerApplicationService.GetAsync(match.AwayTeamRoster.ToList());
			return MatchMapper.ToDTO(match, homeTeam, awayTeam, homeTeamRoster, awayTeamRoster, TournamentMapper.ToDTO(tournament));
		}
		public async Task<List<MatchShortDTO>> GetFinishedByTournamentAsync(int tournamentId, int page, int pageSize)
		{
			List<Match> matches = await _matchRepository.GetFinishedByTournamentAsync(tournamentId, page, pageSize);
			return await GetMatchDTOsByMatchesAsync(matches);
		}
		public async Task<List<MatchShortDTO>> GetScheduleByTournamentAsync(int tournamentId, int page, int pageSize)
		{
			List<Match> matches = await _matchRepository.GetScheduleByTournamentAsync(tournamentId, page, pageSize);
			return await GetMatchDTOsByMatchesAsync(matches);
		}
		public async Task<List<MatchShortDTO>> GetByDateAsync(DateOnly date, int? tournamentId = null)
		{
			var matches = await _matchRepository.GetByDateAsync(date, tournamentId);
			return await GetMatchDTOsByMatchesAsync(matches);
		}
		public async Task<List<MatchShortDTO>> GetFinishedByTeamAsync(int teamId, int page, int pageSize)
		{
			var matches = await _matchRepository.GetFinishedByTeamAsync(teamId, page, pageSize);
			return await GetMatchDTOsByMatchesAsync(matches);
		}
		public async Task<List<MatchShortDTO>> GetScheduleByTeamAsync(int teamId, int page, int pageSize)
		{
			var matches = await _matchRepository.GetScheduleByTeamAsync(teamId, page, pageSize);
			return await GetMatchDTOsByMatchesAsync(matches);
		}
		private async Task<List<MatchShortDTO>> GetMatchDTOsByMatchesAsync(List<Match> matches)
		{
			var teamIds = matches.SelectMany(m => new[] { m.HomeTeamId, m.AwayTeamId }).ToList();
			var teams = await _teamRepository.GetAsync(teamIds);
			var teamsDTO = teams.Select(TeamMapper.ToDTO).ToDictionary(t => t.Id);

			return matches.Select(m => MatchMapper.ToDTO(m, teamsDTO[m.HomeTeamId], teamsDTO[m.AwayTeamId])).ToList();
		}

		public async Task<bool> IsFinished(int matchId)
		{
			var match = await GetMatchOrThrowAsync(matchId);
			return match.IsMatchFinished();
		}
	}
}
