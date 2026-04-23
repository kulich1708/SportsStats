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
				throw new ArgumentException("Длительность овертайма должна быть положительной");

			if (OvertimesCount.HasValue && OvertimesCount <= 0)
				throw new ArgumentException("Количество овертаймов должно быть положительным");

			if (!GoalEndsOvertime && (!OvertimeDurationSeconds.HasValue || !OvertimesCount.HasValue))
				throw new ArgumentException("Овертайм не может быть бесконечный, если гол не завершает его");
		}

		public bool IsInfiniteOvertime()
			=> !OvertimesCount.HasValue || !OvertimeDurationSeconds.HasValue;
	}
}
