using SportsStats.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments.Rules
{
	public record MatchDurationRules
	{
		public string Name { get; init; }
		public int PeriodsCount { get; init; }               // Количество периодов
		public int PeriodDurationSeconds { get; init; }      // Длительность одного периода
		public bool HasOvertime { get; init; }               // Наличие овертайма
		public int? OvertimeDurationSeconds { get; init; }   // Длительность одного овертайма. Если null, то бесконечное время до первого гола
		public int? OvertimesCount { get; init; }            // Возможное количество овертаймов. Если null, то бесконечно как в плей-офф в хоккее
		public int ShootoutsCount { get; init; }             // Количество булитов. 0, если не предусмотрены
		public bool SuddenDeathOvertime { get; init; }       // Весь овертайм или до победного гола
		public bool IsDrawPossible { get; init; }

		private MatchDurationRules() { }
		public MatchDurationRules(
			string name,
			int periodsCount,
			int periodDurationSeconds,
			bool hasOvertime,
			int? overtimeDurationSeconds,
			int? overtimesCount,
			bool suddenDeathOvertime,
			bool isDrawPossible = false,
			int shootoutsCount = 0)
		{
			Name = name;
			PeriodsCount = periodsCount;
			PeriodDurationSeconds = periodDurationSeconds;
			HasOvertime = hasOvertime;
			OvertimeDurationSeconds = overtimeDurationSeconds;
			OvertimesCount = overtimesCount;
			SuddenDeathOvertime = suddenDeathOvertime;
			IsDrawPossible = isDrawPossible;
			ShootoutsCount = shootoutsCount;


			ValidateRules();
		}

		private void ValidateRules()
		{
			if (PeriodsCount <= 0)
				throw new ArgumentException("Periods count must be positive");

			if (PeriodDurationSeconds <= 0)
				throw new ArgumentException("Period duration must be positive");
			if (OvertimeDurationSeconds.HasValue && OvertimeDurationSeconds <= 0)
				throw new ArgumentException("Overtime duration cannot be negative");

			if (HasOvertime && (!OvertimeDurationSeconds.HasValue || !OvertimesCount.HasValue) && !SuddenDeathOvertime)
				throw new ArgumentException("Overtime must either have a fixed time, or be played until the winning goal");
			if (HasOvertime && !OvertimeDurationSeconds.HasValue && OvertimesCount.HasValue)
				throw new ArgumentException("There cannot be multiple overtimes unless their duration is specified");
			if (HasOvertime && (!OvertimeDurationSeconds.HasValue || !OvertimesCount.HasValue) && ShootoutsCount > 0)
				throw new ArgumentException("Shootouts are impossible with endless overtime");
		}
		public bool IsValidPeriod(int period)
		{
			if (period <= 0)
				return false;

			if (period > PeriodsCount && !HasOvertime)
				return false;

			if (OvertimesCount.HasValue && period > OvertimesCount + PeriodsCount)
				return false;

			return true;
		}
		public bool IsValidTimeInPeriod(int period, int time)
		{
			if (!IsValidPeriod(period)) return false;

			if (time < 0) return false;

			if (period <= PeriodsCount)
				return time < PeriodDurationSeconds;
			if (OvertimeDurationSeconds.HasValue)
				return time < OvertimeDurationSeconds;

			return true;
		}
		public bool DoesGoalEndMatch(int period)
		{
			return IsValidPeriod(period) && period > PeriodsCount && SuddenDeathOvertime;
		}
		public bool IsOvertimePeriod(int period)
		{
			return IsValidPeriod(period) && period > PeriodsCount;
		}

		// Фабричные методы для удобства
		public static MatchDurationRules CreateKHLRules()
		{
			return new MatchDurationRules(
				name: "IIHF",
				periodsCount: 3,
				periodDurationSeconds: 1200,
				hasOvertime: true,
				overtimeDurationSeconds: 300,
				overtimesCount: 1,
				suddenDeathOvertime: true,
				shootoutsCount: 3
			);
		}

		public static MatchDurationRules CreatePlayoffRules()
		{
			return new MatchDurationRules(
				name: "IIHF Playoffs",
				periodsCount: 3,
				periodDurationSeconds: 1200,
				hasOvertime: true,
				overtimeDurationSeconds: 1200,
				overtimesCount: null,
				suddenDeathOvertime: true,
				shootoutsCount: 0
			);
		}
	}
}
