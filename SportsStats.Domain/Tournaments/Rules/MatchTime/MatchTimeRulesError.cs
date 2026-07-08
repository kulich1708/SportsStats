using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments.Rules.MatchTime
{

	public static class MatchTimeRulesError
	{
		// ===== 3100-3109: Периоды =====
		public static readonly ErrorCode PeriodsCountMustBePositive = new(3100, "Количество периодов должно быть положительным");
		public static readonly ErrorCode PeriodDurationMustBePositive = new(3101, "Длительность периода должна быть положительной");

		// ===== 3110-3119: Овертайм =====
		public static readonly ErrorCode OvertimeRulesNotAllowed = new(3110, "Нельзя установить правила овертайма, если он не предусмотрен");
		public static readonly ErrorCode OvertimeRulesRequired = new(3111, "Если предусмотрен овертайм, необходимо установить правила для него");
		public static readonly ErrorCode OvertimeDurationMustBePositive = new(3112, "Длительность овертайма должна быть положительной");
		public static readonly ErrorCode OvertimesCountMustBePositive = new(3113, "Количество овертаймов должно быть положительным");
		public static readonly ErrorCode InfiniteOvertimeRequiresGoalEnd = new(3114, "Бесконечный овертайм возможен только если гол завершает игру");
		public static readonly ErrorCode InfiniteOvertimeWithShootout = new(3115, "Установлены буллиты, но до них никогда не дойдёт из-за бесконечного овертайма");
		public static readonly ErrorCode DrawNotAllowedWithInfiniteOvertimeWithoutShootout = new(3116, "Гол в овертайме не завершает игру, буллиты не предусмотрены, а ничья запрещена");

		// ===== 3120-3129: Буллиты =====
		public static readonly ErrorCode ShootoutRulesNotAllowed = new(3120, "Нельзя установить правила буллитов, если они не предусмотрены");
		public static readonly ErrorCode ShootoutRulesRequired = new(3121, "Если предусмотрены буллиты, необходимо установить правила для них");

		// ===== 3130-3139: Ничья =====
		public static readonly ErrorCode DrawNotAllowedWithoutOvertimeOrShootout = new(3130, "Ничья запрещена, но не предусмотрены овертайм или буллиты");
	}
}
