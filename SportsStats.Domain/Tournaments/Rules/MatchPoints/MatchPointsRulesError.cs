using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments.Rules.MatchPoints
{
	public class MatchPointsRulesError
	{
		public const string OvertimePointsRequired = "Для овертайма необходимо указать количество очков за победу и поражение";
		public const string ShootoutPointsRequired = "Для буллитов необходимо указать количество очков за победу и поражение";
		public const string DrawPointsRequired = "При разрешённой ничье необходимо указать количество очков за ничью";
	}
}
