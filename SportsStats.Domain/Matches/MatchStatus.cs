using SportsStats.Domain.Tournaments;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Matches
{
	public enum MatchStatus
	{
		Waiting,
		InProgress,
		Finished
	}

	public static class MatchStatusText
	{
		private static readonly Dictionary<MatchStatus, string> StatusDescription = new()
		{
			[MatchStatus.Waiting] = "В ожидании",
			[MatchStatus.InProgress] = "Начат",
			[MatchStatus.Finished] = "Закончен",
		};
		public static string GetDescription(this MatchStatus statusType)
		{
			return StatusDescription.TryGetValue(statusType, out var text) ? text : string.Empty;
		}

		private static readonly Dictionary<MatchStatus, string> NextStatusActionDescription = new()
		{
			[MatchStatus.Waiting] = "Начать матч",
			[MatchStatus.InProgress] = "Закончить матч",
		};
		public static string GetNextActionDescription(this MatchStatus statusType)
		{
			return NextStatusActionDescription.TryGetValue(statusType, out var text) ? text : string.Empty;
		}
	}
}
