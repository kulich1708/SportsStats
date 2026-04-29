using SportsStats.Domain.Tournaments.Rules.MatchTime;
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
				throw new ArgumentException("Необходимо заполнить правила длины матча");
			if (MatchRosterRules == null)
				throw new ArgumentException("Необходимо заполнить правила заявки игроков на матч");
			if (MatchPointsRules == null)
				throw new ArgumentException("Необходимо заполнить правила начисления очков");

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
