using SportsStats.Domain.Matches;
using SportsStats.Domain.Tournaments;
using SportsStats.Domain.Tournaments.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Services
{
	public class MatchCreationService : IMatchCreationService
	{
		private readonly ITournamentRepository _tournamentRepository;
		private readonly IMatchRepository _matchRepository;

		public MatchCreationService(ITournamentRepository tournamentRepository, IMatchRepository matchRepository)
		{
			_tournamentRepository = tournamentRepository;
			_matchRepository = matchRepository;
		}

		public Match CreateMatch(int tournamentId, int homeTeamId, int awayTeamId)
		{
			Tournament tournament = GetTournamentOrThrow(tournamentId);
			if (!tournament.IsRegistration() && !tournament.IsInProgress())
				throw new ArgumentException("Нельзя создать матч в турнире, который ещё не открыт");
			if (!IsTeamInTournament(tournament, homeTeamId))
				throw new ArgumentException("Домашняя команда не заявлена на турнир");
			if (!IsTeamInTournament(tournament, awayTeamId))
				throw new ArgumentException("Гостевая команда не заявлена на турнир");

			Match match = new Match(tournamentId, homeTeamId, awayTeamId, tournament.TournamentRules);

			return _matchRepository.Save(match);

		}
		private Tournament GetTournamentOrThrow(int tournamentId)
		{
			Tournament tournament = _tournamentRepository.FindById(tournamentId)
				?? throw new ArgumentException("Турнир с таким Id не существует");

			return tournament;
		}
		private bool IsTeamInTournament(Tournament tournament, int teamId) => tournament.TeamsId.Contains(teamId);
	}
}
