using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Tournaments.DTOs.Shared
{
	public record TournamentRulesDTO(
		MatchDurationRulesDTO MatchDurationRules,
		MatchRosterRulesDTO MatchRosterRules,
		MatchPointsRulesDTO MatchPointsRules
	);
}
