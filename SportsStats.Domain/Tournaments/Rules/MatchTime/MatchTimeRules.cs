using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments.Rules.MatchTime
{
	public record MatchTimeRules
	{
		public int PeriodsCount { get; init; }               // Количество периодов
		public int PeriodDurationSeconds { get; init; }      // Длительность одного периода
		public bool IsDrawPossible { get; init; }            // Возможность ничьи
		public bool HasOvertime { get; init; }               // Наличие овертайма
		public bool HasShootout { get; init; }               // Наличие буллитов
		public MatchOvertimeRules? OvertimeRules { get; init; }
		public MatchShootoutRules? ShootoutRules { get; init; }

		private MatchTimeRules() { }
		public MatchTimeRules(
			int periodsCount,
			int periodDurationSeconds,
			bool isDrawPossible,
			bool hasOvertime,
			bool hasShootout,
			MatchOvertimeRules? overtimeRules = null,
			MatchShootoutRules? shootoutRules = null)
		{
			PeriodsCount = periodsCount;
			PeriodDurationSeconds = periodDurationSeconds;
			IsDrawPossible = isDrawPossible;
			HasOvertime = hasOvertime;
			HasShootout = hasShootout;
			OvertimeRules = overtimeRules;
			ShootoutRules = shootoutRules;

			ValidateRules();
		}

		private void ValidateRules()
		{
			if (PeriodsCount <= 0)
				throw new ArgumentException("Количество периодов должно быть положительным");

			if (PeriodDurationSeconds <= 0)
				throw new ArgumentException("Длительность периода должна быть положительной");

			if (!HasOvertime && OvertimeRules != null)
				throw new ArgumentException("Нельзя установить правила овертайма, если он не предусмотрен");

			if (!HasShootout && ShootoutRules != null)
				throw new ArgumentException("Нельзя установить правила буллитов, если они не предусмотрены");

			if (HasOvertime && OvertimeRules == null)
				throw new ArgumentException("Если предусмотрен овертайм, необходимо установить правила для него");

			if (HasShootout && ShootoutRules == null)
				throw new ArgumentException("Если предусмотрены буллиты, необходимо установить правила для них");

			if (HasOvertime && OvertimeRules!.IsInfiniteOvertime() && HasShootout)
				throw new ArgumentException("Установлено наличие буллитов, но до них никогда не дойдёт из-за бесконечного овертайма");

			if (!IsDrawPossible && !HasOvertime && !HasShootout)
				throw new ArgumentException("Ничья запрещена и при этом не предусмотрены овертайм или буллиты");
			if (!IsDrawPossible && HasOvertime && !OvertimeRules!.GoalEndsOvertime && !HasShootout)
				throw new ArgumentException("Гол в овертайме не завершает его, значит после овертайма может быть равный счёт, но буллиты не предусмотрены, а ничья запрещена");
		}
		public bool IsValidPeriod(int period)
		{
			if (period <= 0)
				return false;

			if (period > PeriodsCount && !HasOvertime)
				return false;

			if (HasOvertime && OvertimeRules!.OvertimesCount.HasValue && period > OvertimeRules.OvertimesCount + PeriodsCount)
				return false;

			return true;
		}
		public bool IsValidTimeInPeriod(int period, int time)
		{
			if (!IsValidPeriod(period))
				return false;

			if (time < 0)
				return false;

			if (IsRegularPeriod(period) && time >= PeriodDurationSeconds)
				return false;

			if (IsOvertimePeriod(period) && OvertimeRules!.OvertimeDurationSeconds.HasValue && time >= OvertimeRules!.OvertimeDurationSeconds)
				return false;

			return true;
		}
		public bool DoesGoalEndMatch(int period)
		{
			return IsOvertimePeriod(period) && OvertimeRules!.GoalEndsOvertime;
		}
		public bool IsOvertimePeriod(int period)
			=> IsValidPeriod(period) && period > PeriodsCount;
		private bool IsRegularPeriod(int period)
			=> IsValidPeriod(period) && period <= PeriodsCount;
	}
}
