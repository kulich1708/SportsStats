using SportsStats.Application.Tournaments.DTOs.Responses;
using SportsStats.Application.Tournaments.DTOs.Shared;
using SportsStats.Domain.Matches;
using SportsStats.Domain.Shared;
using SportsStats.Domain.Statistics;
using SportsStats.Domain.Teams;
using SportsStats.Domain.Tournaments;
using SportsStats.Domain.Tournaments.Rules;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace SportsStats.Application.Tournaments
{
	public class TournamentApplicationService(
		ITournamentRepository tournamentRepository,
		ITimeProvider timeProvider,
		ITeamRepository teamRepository,
		ITeamStatsRepository teamStatsRepository,
		IMatchRepository matchRepository)
	{
		private readonly ITournamentRepository _tournamentRepository = tournamentRepository;
		private readonly ITimeProvider _timeProvider = timeProvider;
		private readonly ITeamRepository _teamRepository = teamRepository;
		private readonly ITeamStatsRepository _teamStatsRepository = teamStatsRepository;
		private readonly IMatchRepository _matchRepository = matchRepository;

		private async Task<Tournament> GetTournamentOrThrowAsync(int tournamentId)
		{
			Tournament tournament = await _tournamentRepository.GetAsync(tournamentId)
				?? throw new ArgumentException($"Не существует туринра с id {tournamentId}");

			return tournament;
		}
		private async Task<Tournament> UpdateAndSaveAsync(int tournamentId, Action<Tournament> action)
		{
			Tournament tournament = await GetTournamentOrThrowAsync(tournamentId);

			action(tournament);

			await _tournamentRepository.SaveChangesAsync();
			return tournament;
		}
		public async Task<int> CreateAsync(string name)
		{
			Tournament tournament = new(name);

			await _tournamentRepository.AddAsync(tournament);
			await _tournamentRepository.SaveChangesAsync();

			return tournament.Id;
		}
		public async Task StartAsync(int tournamentId, DateTime? startedAt = null)
		{
			await UpdateAndSaveAsync(tournamentId, tournament => tournament.Start(startedAt ?? _timeProvider.GetCurrentTime()));
		}
		public async Task FinishAsync(int tournamentId, DateTime? finishedAt = null)
		{
			int unfinishedMatchCount = await _matchRepository.GetUnfinishedMatchCountAsync(tournamentId);
			DateTime lastMatchFinishedAt = await _matchRepository.GetLastMatchFinishedAtAsync(tournamentId);
			await UpdateAndSaveAsync(
				tournamentId,
				tournament => tournament.Finish(finishedAt ?? _timeProvider.GetCurrentTime(), unfinishedMatchCount, lastMatchFinishedAt));
		}
		public async Task RegistrationAsync(int tournamentId)
		{
			await UpdateAndSaveAsync(tournamentId, tournament => tournament.Registration());
		}
		public async Task RegistrateTeamAsync(int tournamentId, int teamId)
		{
			Team team = await _teamRepository.GetAsync(teamId)
				?? throw new ArgumentException($"Не существует команды с id {teamId}");

			await UpdateAndSaveAsync(tournamentId, tournament => tournament.RegistrateTeam(teamId));

			TeamStats teamStats = new(teamId, tournamentId);
			await _teamStatsRepository.AddAsync(teamStats);
			await _teamStatsRepository.SaveChangesAsync();
		}

		public async Task SetRulesAsync(int tournamentId, TournamentRulesDTO rules)
		{
			await UpdateAndSaveAsync(tournamentId, tournament => tournament.SetRules(TournamentMapper.ToDomain(rules)));
		}

		public async Task<List<TournamentDTO>> GetAllAsync(bool onlyStarted = false)
		{
			var tournaments = await _tournamentRepository.GetAllAsync(onlyStarted);
			var teamIds = tournaments.SelectMany(t => t.TeamsId).Distinct().ToList();
			var teams = await _teamRepository.GetAsync(teamIds);
			return tournaments.Select(t => TournamentMapper.ToDTO(t, teams.Where(team => t.TeamsId.Contains(team.Id)).ToList())).ToList();
		}
		public async Task<TournamentDTO?> GetAsync(int tournamentId)
		{
			var tournament = await _tournamentRepository.GetAsync(tournamentId);
			var teams = await _teamRepository.GetAllAsync(tournamentId);
			return tournament == null ? null : TournamentMapper.ToDTO(tournament, teams);
		}
		public async Task<List<TournamentShortDTO>> GetActiveByDateAsync(DateOnly date)
		{
			var tournaments = await _tournamentRepository.GetActiveByDateAsync(date);
			return tournaments.Select(TournamentMapper.ToDTO).ToList();
		}
	}
}