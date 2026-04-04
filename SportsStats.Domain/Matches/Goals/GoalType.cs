using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Matches.Goals
{
	public enum HockeyGoalStrengthType
	{
		EvenStrength,
		PowerPlay,
		Shorthanded,
	}
	public enum HockeyGoalNetType
	{
		EmptyNet
	}

	public static class HockeyGoalTypeText
	{
		private static readonly Dictionary<HockeyGoalStrengthType, string> StrengthDescription = new()
		{
			[HockeyGoalStrengthType.EvenStrength] = "В равных составах",
			[HockeyGoalStrengthType.PowerPlay] = "В большинстве",
			[HockeyGoalStrengthType.Shorthanded] = "В меньшенстве",
		};
		public static string GetDescription(this HockeyGoalStrengthType strengthType)
		{
			return StrengthDescription.TryGetValue(strengthType, out var text) ? text : string.Empty;
		}
		public static Dictionary<HockeyGoalNetType, string> NetDescription = new()
		{
			[HockeyGoalNetType.EmptyNet] = "В пустые ворота"
		};
		public static string GetDescription(this HockeyGoalNetType netType)
		{
			return NetDescription.TryGetValue(netType, out var text) ? text : string.Empty;
		}

	}
}
