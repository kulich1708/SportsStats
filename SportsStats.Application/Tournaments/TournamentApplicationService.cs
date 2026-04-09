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

		private async Task<Tournament> GetTournamentOrThrow(int tournamentId)
		{
			Tournament tournament = await _tournamentRepository.FindById(tournamentId)
				?? throw new ArgumentException($"Не существует туринра с id {tournamentId}");

			return tournament;
		}
		private async Task<Tournament> DoSomething(int tournamentId, Action<Tournament> action)
		{
			Tournament tournament = await GetTournamentOrThrow(tournamentId);

			action(tournament);

			await _tournamentRepository.SaveChangesAsync();
			return tournament;
		}
		public async Task<Tournament> Create(string name)
		{
			Tournament tournament = new(name);

			await _tournamentRepository.AddAsync(tournament);
			await _tournamentRepository.SaveChangesAsync();

			return tournament;
		}
		public async Task Start(int tournamentId)
		{
			await DoSomething(tournamentId, tournament => tournament.Start(_timeProvider.GetCurrentTime()));
		}
		public async Task Finish(int tournamentId)
		{
			await DoSomething(tournamentId, tournament => tournament.Finish(_timeProvider.GetCurrentTime()));
		}
		public async Task RegistrateTeam(int tournamentId, int teamId)
		{
			Team team = await _teamRepository.FindById(teamId)
				?? throw new ArgumentException($"Не существует команды с id {teamId}");

			await DoSomething(tournamentId, tournament => tournament.RegistrateTeam(teamId));
		}
		public async Task SetStatus(int tournamentId, TournamentStatus status)
		{
			await DoSomething(tournamentId, tournament => tournament.SetStatus(status));
		}

		// Временно через фабрику
		public async Task SetRules(int tournamentId)
		{
			await DoSomething(tournamentId, tournament => tournament.SetRules(TournamentRules.CreateKHLRules()));
		}
	}
}