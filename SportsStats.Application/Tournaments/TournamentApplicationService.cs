using SportsStats.Domain.Shared;
using SportsStats.Domain.Teams;
using SportsStats.Domain.Tournaments;
using SportsStats.Domain.Tournaments.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Tournaments
{
	public class TournamentApplicationService
	{
		private ITournamentRepository _tournamentRepository;
		private ITimeProvider _timeProvider;
		private ITeamRepository _teamRepository;
		public TournamentApplicationService(
			ITournamentRepository tournamentRepository,
			ITimeProvider timeProvider,
			ITeamRepository teamRepository)
		{
			_tournamentRepository = tournamentRepository;
			_timeProvider = timeProvider;
			_teamRepository = teamRepository;
		}
		private Tournament GetTournamentOrThrow(int tournamentId)
		{
			Tournament tournament = _tournamentRepository.FindById(tournamentId)
				?? throw new ArgumentException($"Не существует туринра с id {tournamentId}");

			return tournament;
		}
		private Tournament DoSomething(int tournamentId, Action<Tournament> action)
		{
			Tournament tournament = GetTournamentOrThrow(tournamentId);

			action(tournament);

			return _tournamentRepository.Save(tournament);
		}
		public Tournament Create(string name)
		{
			Tournament tournament = new Tournament(name);

			return _tournamentRepository.Save(tournament);
		}
		public void Start(int tournamentId)
		{
			DoSomething(tournamentId, tournament => tournament.Start(_timeProvider.GetCurrentTime()));
		}
		public void Finish(int tournamentId)
		{
			DoSomething(tournamentId, tournament => tournament.Finish(_timeProvider.GetCurrentTime()));
		}
		public void RegistrateTeam(int tournamentId, int teamId)
		{
			Tournament tournament = GetTournamentOrThrow(tournamentId);
			Team team = _teamRepository.FindById(teamId)
				?? throw new ArgumentException($"Не существует команды с id {teamId}");

			tournament.RegistrateTeam(teamId);

			_tournamentRepository.Save(tournament);
		}
		public void SetStatus(int tournamentId, TournamentStatus status)
		{
			Tournament tournament = GetTournamentOrThrow(tournamentId);
			tournament.SetStatus(status);
			_tournamentRepository.Save(tournament);
		}

		// Временно через фабрику
		public void SetRules(int tournamentId)
		{
			DoSomething(tournamentId, tournament => tournament.SetRules(TournamentRules.CreateKHLRules()));
		}
	}
}