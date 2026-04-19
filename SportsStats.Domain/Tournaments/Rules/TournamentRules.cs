using SportsStats.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments.Rules
{
	public record TournamentRules
	{

		public MatchDurationRules MatchDurationRules { get; private set; }
		public MatchRosterRules MatchRosterRules { get; private set; }
		public MatchPointsRules MatchPointsRules { get; private set; }

		private TournamentRules() { }
		public TournamentRules(MatchDurationRules matchDurationRules, MatchRosterRules matchRosterRules, MatchPointsRules matchPointsRules)
		{
			MatchDurationRules = matchDurationRules ?? throw new ArgumentNullException();
			MatchRosterRules = matchRosterRules ?? throw new ArgumentNullException();
			MatchPointsRules = matchPointsRules ?? throw new ArgumentNullException();
		}

		public bool HasRules() => MatchDurationRules != null
							   && MatchPointsRules != null
							   && MatchRosterRules != null;
		public static TournamentRules CreateKHLRules()
		{
			var durationRules = MatchDurationRules.CreateKHLRules();
			var rosterRules = MatchRosterRules.CreateKHLRules();
			var pointsRules = MatchPointsRules.CreateKHLRules();
			return new TournamentRules(durationRules, rosterRules, pointsRules);
		}
	}
}
