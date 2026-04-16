using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Players
{
	public enum PositionType
	{
		LeftWinger,
		RightWinger,
		Center,
		LeftDefenseman,
		RightDefenseman,
		Goalie
	}
	public static class PositionTypeText
	{
		private static readonly Dictionary<PositionType, string> PositionDescription = new()
		{
			[PositionType.LeftWinger] = "Левый нападающий",
			[PositionType.RightWinger] = "Правый нападающий",
			[PositionType.Center] = "Центральный нападающий",
			[PositionType.LeftDefenseman] = "Левый защитник",
			[PositionType.RightDefenseman] = "Правый защитник",
			[PositionType.Goalie] = "Вратарь",
		};
		public static string GetDescription(this PositionType position)
		{
			return PositionDescription.TryGetValue(position, out var text) ? text : string.Empty;
		}
	}
}
