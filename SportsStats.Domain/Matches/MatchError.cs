using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Matches
{
	public static class MatchError
	{
		// ===== 1000-1009: Периоды =====
		public static readonly ErrorCode PeriodAlreadyFinished = new(1000, "Период уже завершён");
		public static readonly ErrorCode PeriodAlreadyStarted = new(1001, "Период уже начат");

		// ===== 1010-1019: Статусы матча =====
		public static readonly ErrorCode TeamCannotPlayItself = new(1010, "Команда не может играть сама с собой");
		public static readonly ErrorCode MatchAlreadyStarted = new(1011, "Нельзя начать матч, который уже не в ожидании");
		public static readonly ErrorCode FinishMatchNotInProgress = new(1012, "Нельзя завершить матч, который ещё не начат или уже закончен");
		public static readonly ErrorCode MatchNotInProgress = new(1013, "Матч ещё не начат или уже завершён");
		public static readonly ErrorCode TeamNotInMatch = new(1014, "Команда не участвует в этом матче");
		public static readonly ErrorCode TeamNotParticipatingInMatch = new(1015, "Команда не участвует в этом матче");

		// ===== 1020-1029: Время матча =====
		public static readonly ErrorCode MatchFinishTimeCannotBeBeforeStart = new(1020, "Время завершения матча ({0}) не может быть раньше времени начала ({1})");
		public static readonly ErrorCode InvalidTimeForPeriod = new(1021, "Указанное время не соответствует текущему периоду");
		public static readonly ErrorCode CannotFinishInfiniteOvertimeWithDraw = new(1022, "Нельзя завершить бесконечный овертайм при равном счёте");

		// ===== 1030-1039: Голы =====
		public static readonly ErrorCode GoalNotFoundInMatch = new(1030, "Гол не найден в событиях этого матча");
		public static readonly ErrorCode GoalScorerNotInTeamRoster = new(1031, "Автор гола не заявлен за команду, которая забила гол");
		public static readonly ErrorCode FirstAssistantNotInTeamRoster = new(1032, "Первый ассистент не заявлен за команду, которая забила гол");
		public static readonly ErrorCode SecondAssistantNotInTeamRoster = new(1033, "Второй ассистент не заявлен за команду, которая забила гол");
		public static readonly ErrorCode GoalCanOnlyBeAddedToActiveMatch = new(1034, "Нельзя добавить гол в матч, который сейчас не идёт");
		public static readonly ErrorCode ScoredTeamNotInMatch = new(1035, "Нельзя назначить забившей команду, которая не участвует в матче");
		public static readonly ErrorCode PlayerNotInRoster = new(1036, "Нельзя назначить автором гола игрока, которого нет в заявке команды");

		// ===== 1040-1049: Игроки =====
		public static readonly ErrorCode PlayerAlreadyAdded = new(1040, "Нельзя добавить игрока дважды");
		public static readonly ErrorCode CannotAddPlayerAfterMatchStart = new(1041, "Нельзя добавить игрока после начала матча");
	}
}
