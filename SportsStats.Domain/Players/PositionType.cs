using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Players
{
	public enum PositionType
	{
		Forward,
		Winger,
		LeftWinger,
		RightWinger,
		Center,
		Defenseman,
		LeftDefenseman,
		RightDefenseman,
		Goalie
	}
	public static class PositionTypeText
	{
		private static readonly Dictionary<PositionType, string> _positionDescription = new()
		{
			[PositionType.Forward] = "Нападающий",
			[PositionType.Winger] = "Крайний нападающий",
			[PositionType.LeftWinger] = "Левый нападающий",
			[PositionType.RightWinger] = "Правый нападающий",
			[PositionType.Center] = "Центральный нападающий",
			[PositionType.Defenseman] = "Защитник",
			[PositionType.LeftDefenseman] = "Левый защитник",
			[PositionType.RightDefenseman] = "Правый защитник",
			[PositionType.Goalie] = "Вратарь",
		};
		public static IReadOnlyDictionary<PositionType, string> PositionDescription => _positionDescription;
		public static string GetDescription(this PositionType position)
		{
			return _positionDescription.TryGetValue(position, out var text) ? text : string.Empty;
		}
	}
}
