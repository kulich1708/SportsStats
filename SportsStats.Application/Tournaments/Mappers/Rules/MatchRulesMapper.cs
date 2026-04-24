using SportsStats.Application.Tournaments.DTOs.Shared;
using SportsStats.Domain.Tournaments.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Tournaments.Mappers.Rules
{
	public class MatchRulesMapper
	{
		public static TournamentRulesDTO ToDTO(TournamentRules rules) => new(
			MatchTimeRulesMapper.ToDTO(rules.MatchTimeRules),
			MatchRosterRulesMapper.ToDTO(rules.MatchRosterRules),
			MatchPointsRulesMapper.ToDTO(rules.MatchPointsRules)
		);
		public static TournamentRules ToDomain(TournamentRulesDTO rules) => new(
			MatchTimeRulesMapper.ToDomain(rules.MatchTimeRules),
			MatchRosterRulesMapper.ToDomain(rules.MatchRosterRules),
			MatchPointsRulesMapper.ToDomain(rules.MatchPointsRules)
		);
	}
}
