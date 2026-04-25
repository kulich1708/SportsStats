using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Tournaments.DTOs.Shared.Rules.MatchTime
{
	public record MatchOvertimeRulesDTO(
		int? OvertimesCount,
		int? OvertimeDurationSeconds,
		bool GoalEndsOvertime
	);
}
