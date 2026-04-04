using SportsStats.Domain.Tournaments;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Shared
{
	public interface ITournamentDataProvider
	{
		public TimeBasedTournamentRules GetTimeBasedTournamentRules(int tournamentId);
	}
}
