using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments.Rules.MatchRoster
{
	public class MatchRosterRulesError
	{
		public const string PlayersCountCannotBeNegative = "Количество игроков на любой позиции не может быть отрицательным";
		public const string MinPlayersMustBeAtLeastOne = "Минимальное количество игроков должно быть не менее 1";
		public const string MinCannotExceedMax = "Минимальное количество игроков на позиции не может превышать максимальное";
		public const string MinPlayersByPositionExceedsMaxTotal = "Сумма минимальных составов по позициям превышает максимальное количество игроков";
		public const string MaxPlayersByPositionBelowMinTotal = "Сумма максимальных составов по позициям меньше минимального количества игроков";
	}
}
