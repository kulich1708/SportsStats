using SportsStats.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments.Rules
{
	public class TournamentRules : BaseEntity
	{

		public int MatchDurationRulesId { get; private set; }
		public int MatchRosterRulesId { get; private set; }

		public TournamentRules(int matchDurationRulesId, int matchRosterRulesId)
		{
			MatchDurationRulesId = matchDurationRulesId;
			MatchRosterRulesId = matchRosterRulesId;
		}
	}
}
