using SportsStats.Domain.Matches;
using System;
using System.Collections.Generic;
using System.Text;
using SportsStats.Domain.Tournaments;
using SportsStats.Domain.Players;
using SportsStats.Domain.Teams;

namespace SportsStats.Domain.Services
{
	public interface IMatchService
	{
		public Match CreateMatch(Tournament tournament, int homeTeamId, int awayTeamId);
		public void Start(Match match, List<Player> homeTeamRoster, List<Player> awayTeamRoster, Team homeTeam, Team awayTeam, DateTime startedAt);
	}
}
