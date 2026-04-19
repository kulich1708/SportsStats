using SportsStats.Domain.Matches;
using SportsStats.Domain.Players;
using SportsStats.Domain.Teams;
using SportsStats.Domain.Tournaments;
using SportsStats.Domain.Tournaments.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Services
{
	public interface IMatchService
	{
		public Match CreateMatch(Tournament tournament, int homeTeamId, int awayTeamId, DateTime scheduledAt, TournamentRules rules);
		public void Start(Match match, Tournament tournament, List<Player> homeTeamRoster, List<Player> awayTeamRoster, Team homeTeam, Team awayTeam, DateTime startedAt);
	}
}
