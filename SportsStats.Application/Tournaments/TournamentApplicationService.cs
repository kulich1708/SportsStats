using SportsStats.Application.Matches;
using SportsStats.Application.Matches.DTOs.Responses;
using SportsStats.Application.Tournaments.DTOs.Responses;
using SportsStats.Application.Tournaments.DTOs.Shared;
using SportsStats.Application.Tournaments.Mappers;
using SportsStats.Application.Tournaments.Mappers.Rules;
using SportsStats.Domain.Matches;
using SportsStats.Domain.Shared;
using SportsStats.Domain.Statistics;
using SportsStats.Domain.Teams;
using SportsStats.Domain.Tournaments;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Tournaments
{
	public class TournamentApplicationService(
		ITournamentRepository tournamentRepository,
		ITimeProvider timeProvider,
		ITeamRepository teamRepository,
		ITeamStatsRepository teamStatsRepository,
		IMatchRepository matchRepository,
		MatchQueriesHandler matchQueriesHandler)
	{
		private readonly ITournamentRepository _tournamentRepository = tournamentRepository;
		private readonly ITimeProvider _timeProvider = timeProvider;
		private readonly ITeamRepository _teamRepository = teamRepository;
		private readonly ITeamStatsRepository _teamStatsRepository = teamStatsRepository;
		private readonly IMatchRepository _matchRepository = matchRepository;
		private readonly MatchQueriesHandler _matchQueriesHandler = matchQueriesHandler;

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
			Tournament tournament = await GetTournamentOrThrowAsync(tournamentId);


			foreach (var teamId in tournament.TeamsId)
			{
				TeamStats teamStats = new(teamId, tournamentId);
				await _teamStatsRepository.AddAsync(teamStats);
			}
			await _teamStatsRepository.SaveChangesAsync();
			tournament.Start(startedAt ?? _timeProvider.GetCurrentTime());
			await _tournamentRepository.SaveChangesAsync();
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
		public async Task SetRegistrationTeamsAsync(int tournamentId, List<int> teamIds)
		{
			await UpdateAndSaveAsync(tournamentId, tournament => tournament.SetRegistrationTeams(teamIds));
		}

		public async Task SetRulesAsync(int tournamentId, TournamentRulesDTO rules)
		{
			Console.WriteLine("Пришло в application: " + rules.MatchPointsRules.DrawPoints);
			Console.WriteLine("Приходит от маппера: " + MatchRulesMapper.ToDomain(rules).MatchPointsRules.DrawPoints);
			await UpdateAndSaveAsync(tournamentId, tournament => tournament.SetRules(MatchRulesMapper.ToDomain(rules)));
		}

		public async Task<List<TournamentShortDTO>> GetAllAsync(int page, int pageSize, string? search = null)
		{
			var tournaments = await _tournamentRepository.GetAllAsync(page, pageSize, search);
			var teamIds = tournaments.SelectMany(t => t.TeamsId).Distinct().ToList();
			var teams = await _teamRepository.GetAsync(teamIds);

			return tournaments.Select(TournamentMapper.ToDTO).ToList();
		}
		public async Task<TournamentDTO?> GetAsync(int tournamentId)
		{
			var tournament = await _tournamentRepository.GetAsync(tournamentId);
			var teams = await _teamRepository.GetByTournamentAsync(tournamentId);
			return tournament == null ? null : TournamentMapper.ToDTO(tournament, teams);
		}
		public async Task<List<TournamentWithMatchesDTO>> GetActiveByDateWithMatchesAsync(DateOnly date)
		{
			var matches = await _matchQueriesHandler.GetByDateAsync(date);

			var matchesInTournaments = matches.ToLookup(m => m.TournamentId);
			var tournamentIds = matchesInTournaments.Select(g => g.Key).ToList();
			var tournaments = await _tournamentRepository.GetAsync(tournamentIds);
			var tournamentsLookup = tournaments.ToDictionary(t => t.Id);

			return matchesInTournaments
				.Select(g => TournamentMapper.ToDTO(tournamentsLookup[g.Key], g.ToList()))
				.ToList();
		}
		public async Task<List<TournamentWithMatchesDTO>> GetByTeamWithFinishedMatchesAsync(int teamId, int page, int pageSize)
		{
			var matches = await _matchQueriesHandler.GetFinishedByTeamAsync(teamId, page, pageSize);
			return await GetTournamentWithMatchesDTOsByMatchesAsync(matches);
		}
		public async Task<List<TournamentWithMatchesDTO>> GetByTeamWithScheduleMatchesAsync(int teamId, int page, int pageSize)
		{
			var matches = await _matchQueriesHandler.GetScheduleByTeamAsync(teamId, page, pageSize);
			return await GetTournamentWithMatchesDTOsByMatchesAsync(matches);
		}
		public async Task<List<TournamentWithMatchesDTO>> GetTournamentWithMatchesDTOsByMatchesAsync(List<MatchShortDTO> matches)
		{
			List<List<MatchShortDTO>> matchesInTournaments = new();
			matchesInTournaments.Add([matches[0]]);
			for (int i = 1; i < matches.Count; i++)
			{
				if (matches[i].TournamentId == matches[i - 1].TournamentId)
					matchesInTournaments[^1].Add(matches[i]);
				else
					matchesInTournaments.Add([matches[i]]);
			}

			var tournamentIds = matches.Select(m => m.TournamentId).Distinct().ToList();
			var tournaments = await _tournamentRepository.GetAsync(tournamentIds);
			var tournamentsLookup = tournaments.ToDictionary(t => t.Id);

			return matchesInTournaments
				.Select(g => TournamentMapper.ToDTO(tournamentsLookup[g[0].TournamentId], g))
				.ToList();
		}
		public async Task ChangeGeneralInfoAsync(int id, string name, byte[]? photo, string? photoMime)
		{
			Tournament tournament = await GetTournamentOrThrowAsync(id);
			tournament.SetName(name);
			if (photo != null)
				tournament.SetPhoto(photo, photoMime);

			await _tournamentRepository.SaveChangesAsync();
		}
	}
}
