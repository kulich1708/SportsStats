using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments.Rules.MatchRoster
{
	public static class MatchRosterRulesError
	{
		// ===== 3200-3209: Общие правила состава =====
		public static readonly ErrorCode PlayersCountCannotBeNegative = new(3200, "Количество игроков на любой позиции не может быть отрицательным");
		public static readonly ErrorCode MinPlayersMustBeAtLeastOne = new(3201, "Минимальное количество игроков должно быть не менее 1");
		public static readonly ErrorCode MinCannotExceedMax = new(3202, "Минимальное количество игроков на позиции не может превышать максимальное");
		public static readonly ErrorCode MinPlayersByPositionExceedsMaxTotal = new(3203, "Сумма минимальных составов по позициям превышает максимальное количество игроков");
		public static readonly ErrorCode MaxPlayersByPositionBelowMinTotal = new(3204, "Сумма максимальных составов по позициям меньше минимального количества игроков");
	}
}
