using SportsStats.Application.Tournaments.DTOs.Shared.Rules.MatchTime;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Tournaments.DTOs.Shared
{
	public record TournamentRulesDTO(
		MatchTimeRulesDTO MatchTimeRules,
		MatchRosterRulesDTO MatchRosterRules,
		MatchPointsRulesDTO MatchPointsRules
	);
}
