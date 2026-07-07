using SportsStats.Domain.Matches;
using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
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
		public int? AllPeriodsCount
		{
			get
			{
				if (HasOvertime && !OvertimeRules!.OvertimesCount.HasValue)
					return null;
				return PeriodsCount + (OvertimeRules?.OvertimesCount ?? 0) + (HasShootout ? 1 : 0);
			}
		}
		public int? FirstOvertimePeriod => HasOvertime ? PeriodsCount + 1 : null;
		public int? ShootoutPeriod => HasShootout ? AllPeriodsCount : null;

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
				throw new DomainException(MatchTimeRulesError.PeriodsCountMustBePositive);

			if (PeriodDurationSeconds <= 0)
				throw new DomainException(MatchTimeRulesError.PeriodDurationMustBePositive);

			if (!HasOvertime && OvertimeRules != null)
				throw new DomainException(MatchTimeRulesError.OvertimeRulesNotAllowed);

			if (!HasShootout && ShootoutRules != null)
				throw new DomainException(MatchTimeRulesError.ShootoutRulesNotAllowed);

			if (HasOvertime && OvertimeRules == null)
				throw new DomainException(MatchTimeRulesError.OvertimeRulesRequired);

			if (HasShootout && ShootoutRules == null)
				throw new DomainException(MatchTimeRulesError.ShootoutRulesRequired);

			if (HasOvertime && OvertimeRules!.IsInfiniteOvertime() && HasShootout)
				throw new DomainException(MatchTimeRulesError.InfiniteOvertimeWithShootout);

			if (!IsDrawPossible && !HasOvertime && !HasShootout)
				throw new DomainException(MatchTimeRulesError.DrawNotAllowedWithoutOvertimeOrShootout);
			if (!IsDrawPossible && HasOvertime && !OvertimeRules!.GoalEndsOvertime && !HasShootout)

				throw new DomainException(MatchTimeRulesError.DrawNotAllowedWithInfiniteOvertimeWithoutShootout);
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
		public bool IsRegularPeriod(int period)
			=> IsValidPeriod(period) && period <= PeriodsCount;
		public bool IsShootout(int period)
			=> HasShootout && period == AllPeriodsCount;
		public bool IsOneInfinityOvertime(int period)
			=> !HasShootout && HasOvertime && !OvertimeRules!.OvertimeDurationSeconds.HasValue && IsOvertimePeriod(period);
		public static MatchTimeRules CreateKHLMatchTimeRules()
		{
			var overtimeRules = MatchOvertimeRules.CreateKHLOvertimeRules();
			var shootoutRules = MatchShootoutRules.CreateKHLShootoutRules();
			return new(3, 1200, false, true, false, overtimeRules, shootoutRules);
		}
	}
}
