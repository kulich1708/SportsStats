using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments.Rules.MatchPoints
{
	public static class MatchPointsRulesError
	{
		// ===== 3300-3309: Очки за исходы =====
		public static readonly ErrorCode OvertimePointsRequired = new(3300, "Для овертайма необходимо указать количество очков за победу и поражение");
		public static readonly ErrorCode ShootoutPointsRequired = new(3301, "Для буллитов необходимо указать количество очков за победу и поражение");
		public static readonly ErrorCode DrawPointsRequired = new(3302, "При разрешённой ничье необходимо указать количество очков за ничью");
	}
}
