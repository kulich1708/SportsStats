using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments.Rules.MatchTime
{

	public static class MatchTimeRulesError
	{
		public const string PeriodsCountMustBePositive = "Количество периодов должно быть положительным";
		public const string PeriodDurationMustBePositive = "Длительность периода должна быть положительной";
		public const string OvertimeRulesNotAllowed = "Нельзя установить правила овертайма, если он не предусмотрен";
		public const string ShootoutRulesNotAllowed = "Нельзя установить правила буллитов, если они не предусмотрены";
		public const string OvertimeRulesRequired = "Если предусмотрен овертайм, необходимо установить правила для него";
		public const string ShootoutRulesRequired = "Если предусмотрены буллиты, необходимо установить правила для них";
		public const string InfiniteOvertimeWithShootout = "Установлены буллиты, но до них никогда не дойдёт из-за бесконечного овертайма";
		public const string DrawNotAllowedWithoutOvertimeOrShootout = "Ничья запрещена, но не предусмотрены овертайм или буллиты";
		public const string DrawNotAllowedWithInfiniteOvertimeWithoutShootout = "Гол в овертайме не завершает игру, буллиты не предусмотрены, а ничья запрещена";

		public const string OvertimeDurationMustBePositive = "Длительность овертайма должна быть положительной";
		public const string OvertimesCountMustBePositive = "Количество овертаймов должно быть положительным";
		public const string InfiniteOvertimeRequiresGoalEnd = "Бесконечный овертайм возможен только если гол завершает игру";
	}
}
