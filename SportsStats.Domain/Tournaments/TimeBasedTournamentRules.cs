using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments
{
	public class TimeBasedTournamentRules : TournamentRulesBase
	{
		public int PeriodsCount { get; private set; }               // Количество периодов
		public int PeriodDurationSeconds { get; private set; }      // Длительность одного периода
		public bool HasOvertime { get; private set; }               // Наличие овертайма
		public int? OvertimeDurationSeconds { get; private set; }   // Длительность одного овертайма. Если null, то бесконечное время до первого гола
		public int? OvertimesCount { get; private set; }            // Возможное количество овертаймов. Если null, то бесконечно как в плей-офф в хоккее
		public int ShootoutsCount { get; private set; }             // Количество булитов. 0, если не предусмотрены
		public bool SuddenDeathOvertime { get; private set; }       // Весь овертайм или до победного гола

		public TimeBasedTournamentRules(
			string name,
			int periodsCount,
			int periodDurationSeconds,
			bool hasOvertime,
			int? overtimeDurationSeconds,
			int? overtimesCount,
			bool suddenDeathOvertime,
			int shootoutsCount = 0) : base(name)
		{
			PeriodsCount = periodsCount;
			PeriodDurationSeconds = periodDurationSeconds;
			HasOvertime = hasOvertime;
			OvertimeDurationSeconds = overtimeDurationSeconds;
			SuddenDeathOvertime = suddenDeathOvertime;
			ShootoutsCount = shootoutsCount;

			Validate();
		}

		private void Validate()
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

		// Фабричные методы для удобства
		public static TimeBasedTournamentRules CreateKHLRules()
		{
			return new TimeBasedTournamentRules(
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

		public static TimeBasedTournamentRules CreatePlayoffRules()
		{
			return new TimeBasedTournamentRules(
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

		public static TimeBasedTournamentRules CreateFootballLeagueRules()
		{
			return new TimeBasedTournamentRules(
				name: "Football League",
				periodsCount: 2,
				periodDurationSeconds: 2700,
				hasOvertime: true,
				overtimeDurationSeconds: 900,
				overtimesCount: 2,
				suddenDeathOvertime: false,
				shootoutsCount: 0
			);
		}
	}
}
