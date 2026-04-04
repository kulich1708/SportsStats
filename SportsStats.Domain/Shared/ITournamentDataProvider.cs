using SportsStats.Domain.Tournaments.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Shared
{
	public interface ITournamentDataProvider
	{
		public MatchDurationRules GetTimeBasedTournamentRules(int tournamentId);
	}
}
