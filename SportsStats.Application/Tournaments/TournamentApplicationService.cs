using SportsStats.Application.Tournaments.DTOs.Responses;
using SportsStats.Domain.Shared;
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
		ITeamRepository teamRepository)
	{
		private readonly ITournamentRepository _tournamentRepository = tournamentRepository;
		private readonly ITimeProvider _timeProvider = timeProvider;
		private readonly ITeamRepository _teamRepository = teamRepository;

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
		public async Task StartAsync(int tournamentId)
		{
			await UpdateAndSaveAsync(tournamentId, tournament => tournament.Start(_timeProvider.GetCurrentTime()));
		}
		public async Task FinishAsync(int tournamentId)
		{
			await UpdateAndSaveAsync(tournamentId, tournament => tournament.Finish(_timeProvider.GetCurrentTime()));
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
		}

		// Временно через фабрику
		public async Task SetRulesAsync(int tournamentId)
		{
			await UpdateAndSaveAsync(tournamentId, tournament => tournament.SetRules(TournamentRules.CreateKHLRules()));
		}

		public async Task<List<TournamentDTO>> GetAllAsync(bool onlyStarted = false)
		{
			var tournaments = await _tournamentRepository.GetAllAsync(onlyStarted);
			return tournaments.Select(TournamentMapper.ToDTO).ToList();
		}
		public async Task<TournamentDTO?> GetAsync(int tournamentId)
		{
			var tournament = await _tournamentRepository.GetAsync(tournamentId);
			return tournament == null ? null : TournamentMapper.ToDTO(tournament);
		}
	}
}