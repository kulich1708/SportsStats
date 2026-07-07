using SportsStats.Domain.Tournaments.Rules.MatchTime;
using SportsStats.Domain.Tournaments.Rules.MatchRoster;
using SportsStats.Domain.Tournaments.Rules.MatchPoints;
using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments.Rules
{
	public record TournamentRules
	{

		public MatchTimeRules MatchTimeRules { get; private set; }
		public MatchRosterRules MatchRosterRules { get; private set; }
		public MatchPointsRules MatchPointsRules { get; private set; }

		private TournamentRules() { }
		public TournamentRules(MatchTimeRules matchTimeRules, MatchRosterRules matchRosterRules, MatchPointsRules matchPointsRules)
		{
			MatchTimeRules = matchTimeRules;
			MatchRosterRules = matchRosterRules;
			MatchPointsRules = matchPointsRules;
			ValidateRules();
		}
		private void ValidateRules()
		{
			if (MatchTimeRules == null)
				throw new DomainException(TournamentRulesError.MatchTimeRulesRequired);
			if (MatchRosterRules == null)
				throw new DomainException(TournamentRulesError.MatchRosterRulesRequired);
			if (MatchPointsRules == null)
				throw new DomainException(TournamentRulesError.MatchPointsRulesRequired);

			bool hasOvertime = MatchTimeRules.HasOvertime;
			bool hasShootout = MatchTimeRules.HasShootout;
			bool isDrawPossible = MatchTimeRules.IsDrawPossible;
			MatchPointsRules.ValidateRules(hasOvertime, hasShootout, isDrawPossible);

		}

		public bool HasRules() => MatchTimeRules != null
							   && MatchPointsRules != null
							   && MatchRosterRules != null;

		public static TournamentRules CreateKHLRules()
		{
			var timeRules = MatchTimeRules.CreateKHLMatchTimeRules();
			var rosterRules = MatchRosterRules.CreateKHLRules();
			var pointsRules = MatchPointsRules.CreateKHLPointsRules();

			return new(timeRules, rosterRules, pointsRules);
		}
	}
}
