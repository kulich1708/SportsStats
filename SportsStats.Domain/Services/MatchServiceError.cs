using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Services
{
	public static class MatchServiceError
	{
		// ===== 5000-5009: Создание матча =====
		public static readonly ErrorCode MatchCanOnlyBeCreatedInRegistrationOrStarted = new(5000, "Нельзя создать матч в турнире, который ещё не открыт");
		public static readonly ErrorCode MatchScheduleTimeCannotBeBeforeTournamentStart = new(5001, "Время матча ({0}) не может быть раньше начала турнира ({1})");
		public static readonly ErrorCode HomeTeamNotRegistered = new(5002, "Домашняя команда не заявлена на турнир");
		public static readonly ErrorCode AwayTeamNotRegistered = new(5003, "Гостевая команда не заявлена на турнир");

		// ===== 5010-5019: Старт матча =====
		public static readonly ErrorCode MatchCannotBeStartedInNotStartedTournament = new(5010, "Нельзя начать матч в неначатом турнире");
		public static readonly ErrorCode MatchCannotBeStartedBeforeTournamentStart = new(5011, "Время начала матча ({0}) не может быть раньше начала турнира ({1})");

		// ===== 5020-5029: Состав команды =====
		public static readonly ErrorCode ForwardsCountOutOfRange = new(5020, "Количество нападающих в команде {0} должно быть от {1} до {2}");
		public static readonly ErrorCode DefensemenCountOutOfRange = new(5021, "Количество защитников в команде {0} должно быть от {1} до {2}");
		public static readonly ErrorCode GoaliesCountOutOfRange = new(5022, "Количество вратарей в команде {0} должно быть от {1} до {2}");
		public static readonly ErrorCode PlayersCountOutOfRange = new(5023, "Количество игроков в команде {0} должно быть от {1} до {2}");
	}
}
