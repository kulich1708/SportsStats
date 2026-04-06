using SportsStats.Domain.Shared;
using SportsStats.Domain.Teams;
using SportsStats.Domain.Tournaments;
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
		public void Start(int tournamentId)
		{
			Tournament tournament = GetTournamentOrThrow(tournamentId);

			tournament.Start(_timeProvider.GetCurrentTime());

			_tournamentRepository.Save(tournament);
		}
		public void Finish(int tournamentId)
		{
			Tournament tournament = GetTournamentOrThrow(tournamentId);

			tournament.Finish(_timeProvider.GetCurrentTime());

			_tournamentRepository.Save(tournament);
		}
		public void RegistrateTeam(int tournamentId, int teamId)
		{
			Tournament tournament = GetTournamentOrThrow(tournamentId);
			Team team = _teamRepository.FindById(teamId)
				?? throw new ArgumentException($"Не существует команды с id {teamId}");

			tournament.RegistrateTeam(teamId);

			_tournamentRepository.Save(tournament);
		}
	}
}