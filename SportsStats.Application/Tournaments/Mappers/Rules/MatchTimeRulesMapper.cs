using SportsStats.Application.Tournaments.DTOs.Shared.Rules.MatchTime;
using SportsStats.Domain.Tournaments.Rules.MatchTime;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Tournaments.Mappers.Rules
{
	public class MatchTimeRulesMapper
	{
		public static MatchTimeRulesDTO ToDTO(MatchTimeRules rules) => new(
			rules.PeriodsCount,
			rules.PeriodDurationSeconds,
			rules.IsDrawPossible,
			rules.HasOvertime,
			rules.HasShootout,
			rules.OvertimeRules == null ? null : ToDTO(rules.OvertimeRules),
			rules.ShootoutRules == null ? null : ToDTO(rules.ShootoutRules)
		);

		public static MatchTimeRules ToDomain(MatchTimeRulesDTO rules) => new(
			rules.PeriodsCount,
			rules.PeriodDurationSeconds,
			rules.IsDrawPossible,
			rules.HasOvertime,
			rules.HasShootout,
			rules.OvertimeRules == null ? null : ToDomain(rules.OvertimeRules),
			rules.ShootoutRules == null ? null : ToDomain(rules.ShootoutRules)
		);
		private static MatchOvertimeRulesDTO ToDTO(MatchOvertimeRules rules) => new(
			rules.OvertimesCount,
			rules.OvertimeDurationSeconds,
			rules.GoalEndsOvertime
		);
		private static MatchOvertimeRules ToDomain(MatchOvertimeRulesDTO rules) => new(
			rules.OvertimesCount,
			rules.OvertimeDurationSeconds,
			rules.GoalEndsOvertime
		);

		private static MatchShootoutRulesDTO ToDTO(MatchShootoutRules rules) => new();
		private static MatchShootoutRules ToDomain(MatchShootoutRulesDTO rules) => new();
	}
}
