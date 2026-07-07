using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Tournaments.DTOs.Shared
{
	public record MatchRosterRulesDTO(
		int MinPlayers,
		int MaxPlayers,
		int MinForwards,
		int MaxForwards,
		int MinDefensemans,
		int MaxDefensemans,
		int MinGoalies,
		int MaxGoalies
	);
}
