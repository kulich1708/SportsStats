using SportsStats.Application.Matches.DTOs.Requests;
using SportsStats.Application.Tournaments.Mappers;
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
	public class MatchLifecycleService(IPlayerRepository playerRepository,
		ITournamentRepository tournamentRepository, IMatchRepository matchRepository,
		ITeamRepository teamRepository,
		ITimeProvider timeProvider,
		IMatchService matchService) : MatchUseCaseBase(matchRepository)
	{
		private readonly IPlayerRepository _playerRepository = playerRepository;
		private readonly ITournamentRepository _tournamentRepository = tournamentRepository;
		private readonly IMatchRepository _matchRepository = matchRepository;
		private readonly ITeamRepository _teamRepository = teamRepository;
		private readonly ITimeProvider _timeProvider = timeProvider;
		private readonly IMatchService _matchService = matchService;


		public async Task<int> CreateAsync(int tournamentId, int homeTeamId, int awayTeamId, DateTime scheduledAt)
		{
			Tournament tournament = await _tournamentRepository.GetAsync(tournamentId)
				?? throw new ArgumentException("Нет турнира с таким Id");

			Match match = _matchService.CreateMatch(tournament, homeTeamId, awayTeamId, scheduledAt, tournament.TournamentRules);
			await _matchRepository.AddAsync(match);
			await _matchRepository.SaveChangesAsync();
			return match.Id;
		}
		public async Task StartAsync(int matchId, DateTime? startedAt = null)
		{
			Match match = await GetMatchOrThrowAsync(matchId);
			Tournament tournament = await _tournamentRepository.GetAsync(match.TournamentId);
			Team homeTeam = await _teamRepository.GetAsync(match.HomeTeamId);
			Team awayTeam = await _teamRepository.GetAsync(match.AwayTeamId);

			List<Player> homeTeamRoster = await _playerRepository.GetAsync(match.HomeTeamRoster.ToList());
			List<Player> awayTeamRoster = await _playerRepository.GetAsync(match.AwayTeamRoster.ToList());

			_matchService.Start(match, tournament, homeTeamRoster, awayTeamRoster, homeTeam, awayTeam, startedAt ?? _timeProvider.GetCurrentTime());

			await _matchRepository.SaveChangesAsync();
		}

		public async Task ChangeGeneralInfoAsync(int id, MatchGeneralInfoDTO dto)
		{
			Match match = await GetMatchOrThrowAsync(id);
			match.SetScheduleAt(dto.ScheduleAt);
			await _matchRepository.SaveChangesAsync();
		}
	}
}
