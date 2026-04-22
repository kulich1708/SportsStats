using SportsStats.Application.Tournaments.DTOs.Shared;
using SportsStats.Domain.Tournaments.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Tournaments.Mappers.Rules
{
	public class MatchRosterRulesMapper
	{
		public static MatchRosterRulesDTO ToDTO(MatchRosterRules rules) => new(
			rules.MaxPlayers,
			rules.MinPlayers,
			rules.MinForwards,
			rules.MaxForwards,
			rules.MinDefensemans,
			rules.MaxDefensemans,
			rules.MinGoalies,
			rules.MaxGoalies
		);

		public static MatchRosterRules ToDomain(MatchRosterRulesDTO rules) => new(
			rules.MaxPlayers,
			rules.MinPlayers,
			rules.MinForwards,
			rules.MaxForwards,
			rules.MinDefensemans,
			rules.MaxDefensemans,
			rules.MinGoalies,
			rules.MaxGoalies
		);
	}
}
