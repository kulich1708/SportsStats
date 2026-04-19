using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Matches.Goals
{
	public enum GoalStrengthType
	{
		EvenStrength,
		PowerPlay,
		Shorthanded,
	}
	public enum GoalNetType
	{
		EmptyNet
	}

	public static class GoalTypeText
	{
		private static readonly Dictionary<GoalStrengthType, string> StrengthDescription = new()
		{
			[GoalStrengthType.EvenStrength] = "В равных составах",
			[GoalStrengthType.PowerPlay] = "В большинстве",
			[GoalStrengthType.Shorthanded] = "В меньшенстве",
		};
		public static string GetDescription(this GoalStrengthType strengthType)
		{
			return StrengthDescription.TryGetValue(strengthType, out var text) ? text : string.Empty;
		}
		public static Dictionary<GoalNetType, string> NetDescription = new()
		{
			[GoalNetType.EmptyNet] = "В пустые ворота"
		};
		public static string GetDescription(this GoalNetType netType)
		{
			return NetDescription.TryGetValue(netType, out var text) ? text : string.Empty;
		}
	}
}
