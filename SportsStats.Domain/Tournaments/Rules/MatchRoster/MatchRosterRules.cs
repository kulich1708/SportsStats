using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments.Rules.MatchRoster
{
	public record MatchRosterRules
	{
		public int MinPlayers { get; private set; }
		public int MaxPlayers { get; private set; }
		public int MinForwards { get; private set; }
		public int MaxForwards { get; private set; }
		public int MinDefensemans { get; private set; }
		public int MaxDefensemans { get; private set; }
		public int MinGoalies { get; private set; }
		public int MaxGoalies { get; private set; }

		public MatchRosterRules(int minPlayers = 20, int maxPlayers = 20,
			int minForwards = 12, int maxForwards = 12,
			int minDefensemans = 6, int maxDefensemans = 6,
			int minGoalies = 2, int maxGoalies = 2)
		{
			MinPlayers = minPlayers;
			MaxPlayers = maxPlayers;
			MinForwards = minForwards;
			MaxForwards = maxForwards;
			MinDefensemans = minDefensemans;
			MaxDefensemans = maxDefensemans;
			MinGoalies = minGoalies;
			MaxGoalies = maxGoalies;

			ValidateRoster();
		}
		private void ValidateRoster()
		{
			int[] allPlayers = [MinPlayers, MaxPlayers, MinForwards, MaxForwards, MinDefensemans, MaxDefensemans, MinGoalies, MaxGoalies];
			if (allPlayers.Any(p => p < 0))
				throw new DomainException(MatchRosterRulesError.PlayersCountCannotBeNegative);

			if (MinPlayers < 1)
				throw new DomainException(MatchRosterRulesError.MinPlayersMustBeAtLeastOne);

			if (MinPlayers > MaxPlayers ||
				MinForwards > MaxForwards ||
				MinDefensemans > MaxDefensemans ||
				MinGoalies > MaxGoalies)
				throw new DomainException(MatchRosterRulesError.MinCannotExceedMax);

			int calculatedMinPlayers = MinForwards + MinDefensemans + MinGoalies;
			if (MaxPlayers < calculatedMinPlayers)
				throw new DomainException(MatchRosterRulesError.MinPlayersByPositionExceedsMaxTotal);

			int calculatedMaxPlayers = MaxForwards + MaxDefensemans + MaxGoalies;
			if (MinPlayers > calculatedMaxPlayers)
				throw new DomainException(MatchRosterRulesError.MaxPlayersByPositionBelowMinTotal);
		}

		public static MatchRosterRules CreateKHLRules()
		{
			return new MatchRosterRules(
			minPlayers: 20, maxPlayers: 22,
			minForwards: 12, maxForwards: 13,
			minDefensemans: 6, maxDefensemans: 7,
			minGoalies: 2, maxGoalies: 2);
		}
	}
}
