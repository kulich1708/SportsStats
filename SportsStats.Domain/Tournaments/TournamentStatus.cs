using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments
{
	public enum TournamentStatus
	{
		Draft,
		Registration,
		InProgress,
		Finished
	}
	public static class TournamentStatusText
	{
		private static readonly Dictionary<TournamentStatus, string> StatusDescription = new()
		{
			[TournamentStatus.Draft] = "Создан",
			[TournamentStatus.Registration] = "Открыта регистрация команд",
			[TournamentStatus.InProgress] = "Начат",
			[TournamentStatus.Finished] = "Закончен",
		};
		public static string GetDescription(this TournamentStatus statusType)
		{
			return StatusDescription.TryGetValue(statusType, out var text) ? text : string.Empty;
		}
	}
}
