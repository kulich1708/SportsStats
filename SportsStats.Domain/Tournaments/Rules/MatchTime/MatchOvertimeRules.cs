using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments.Rules.MatchTime
{
	public record MatchOvertimeRules
	{
		public int? OvertimeDurationSeconds { get; init; }   // null = бесконечное время
		public int? OvertimesCount { get; init; }            // null = бесконечное количество
		public bool GoalEndsOvertime { get; init; }


		public MatchOvertimeRules(int? overtimesCount, int? overtimeDurationSeconds, bool goalEndsOvertime)
		{
			OvertimeDurationSeconds = overtimeDurationSeconds;
			OvertimesCount = OvertimeDurationSeconds.HasValue ? overtimesCount : 1;
			GoalEndsOvertime = goalEndsOvertime;

			Validate();
		}

		private void Validate()
		{
			if (OvertimeDurationSeconds.HasValue && OvertimeDurationSeconds <= 0)
				throw new DomainException(MatchTimeRulesError.OvertimeDurationMustBePositive);

			if (OvertimesCount.HasValue && OvertimesCount <= 0)
				throw new DomainException(MatchTimeRulesError.OvertimesCountMustBePositive);

			if (!GoalEndsOvertime && (!OvertimeDurationSeconds.HasValue || !OvertimesCount.HasValue))
				throw new DomainException(MatchTimeRulesError.InfiniteOvertimeRequiresGoalEnd);
		}

		public bool IsInfiniteOvertime()
			=> !OvertimesCount.HasValue || !OvertimeDurationSeconds.HasValue;

		public static MatchOvertimeRules CreateKHLOvertimeRules() => new(1, 300, true);
	}
}
