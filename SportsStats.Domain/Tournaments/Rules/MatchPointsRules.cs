using SportsStats.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments.Rules
{
	public record MatchPointsRules
	{
		public Dictionary<MatchWinType, int> Points = new();
		public MatchPointsRules(int regWin, int otWin, int regLoss, int otLoss, int? draw = null)
		{
			Points.Add(MatchWinType.REGULATION_WIN, regWin);
			Points.Add(MatchWinType.OT_WIN, otWin);
			Points.Add(MatchWinType.REGULATION_LOSS, regLoss);
			Points.Add(MatchWinType.OT_LOSS, otLoss);
			if (draw.HasValue)
				Points.Add(MatchWinType.DRAW, draw.Value);
		}

		public static MatchPointsRules CreateKHLRules()
		{
			return new MatchPointsRules(2, 2, 0, 1);
		}
	}
}
