using SportsStats.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments.Rules
{
	public class TournamentRules
	{

		public MatchDurationRules MatchDurationRules { get; private set; }
		public MatchRosterRules MatchRosterRules { get; private set; }
		public MatchPointsRules MatchPointsRules { get; private set; }

		public TournamentRules(MatchDurationRules matchDurationRules, MatchRosterRules matchRosterRules, MatchPointsRules matchPointsRules)
		{
			MatchDurationRules = matchDurationRules ?? throw new ArgumentNullException();
			MatchRosterRules = matchRosterRules ?? throw new ArgumentNullException();
			MatchPointsRules = matchPointsRules ?? throw new ArgumentNullException();
		}
	}
}
