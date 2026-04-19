using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Tournaments.DTOs.Shared
{
	public record MatchDurationRulesDTO(
		int PeriodsCount,
		int PeriodDurationSeconds,
		bool HasOvertime,
		int? OvertimeDurationSeconds,
		int? OvertimesCount,
		bool SuddenDeathOvertime,
		bool IsDrawPossible,
		int ShootoutsCount
	);
}
