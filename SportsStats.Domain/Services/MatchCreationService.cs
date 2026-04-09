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

		public Match CreateMatch(Tournament tournament, int homeTeamId, int awayTeamId)
		{
			if (!tournament.IsRegistration() && !tournament.IsStarted())
				throw new ArgumentException("Нельзя создать матч в турнире, который ещё не открыт");
			if (!IsTeamInTournament(tournament, homeTeamId))
				throw new ArgumentException("Домашняя команда не заявлена на турнир");
			if (!IsTeamInTournament(tournament, awayTeamId))
				throw new ArgumentException("Гостевая команда не заявлена на турнир");

			Match match = new Match(tournament.Id, homeTeamId, awayTeamId, tournament.TournamentRules);

			return match;

		}
		private bool IsTeamInTournament(Tournament tournament, int teamId) => tournament.TeamsId.Contains(teamId);
	}
}
