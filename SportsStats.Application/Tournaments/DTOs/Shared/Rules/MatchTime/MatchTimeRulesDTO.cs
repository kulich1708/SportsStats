using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Tournaments.DTOs.Shared.Rules.MatchTime
{
	public record MatchTimeRulesDTO(
		int PeriodsCount,
		int PeriodDurationSeconds,
		bool IsDrawPossible,
		bool HasOvertime,
		bool HasShootout,
		MatchOvertimeRulesDTO? OvertimeRules,
		MatchShootoutRulesDTO? ShootoutRules
	);
}
