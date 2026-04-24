using SportsStats.Domain.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Shared.Enums
{
	public enum MatchWinType
	{
		REGULATION_WIN,
		REGULATION_LOSS,
		OT_WIN,
		OT_LOSS,
		SHOOTOUT_WIN,
		SHOOTOUT_LOSS,
		DRAW
	}
	public static class MatchWinTypeText
	{
		private static readonly Dictionary<MatchWinType, string> PositionDescription = new()
		{
			[MatchWinType.REGULATION_WIN] = "Победа в основное время",
			[MatchWinType.REGULATION_LOSS] = "Поражение в основное время",
			[MatchWinType.OT_WIN] = "Победа в овертайме",
			[MatchWinType.OT_LOSS] = "Поражение в овертайме",
			[MatchWinType.SHOOTOUT_WIN] = "Победа в булитах",
			[MatchWinType.SHOOTOUT_LOSS] = "Поражение в булитах",
			[MatchWinType.DRAW] = "Ничья",
		};
		public static string GetDescription(this MatchWinType winType)
		{
			return PositionDescription.TryGetValue(winType, out var text) ? text : string.Empty;
		}
	}
}
