using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Matches
{
	public class MatchError
	{
		public const string PeriodAlreadyFinished = "Период уже завершён";
		public const string PeriodAlreadyStarted = "Период уже начат";

		public const string TeamCannotPlayItself = "Команда не может играть сама с собой";
		public const string MatchAlreadyStarted = "Нельзя начать матч, который уже не в ожидании";
		public const string FinishMatchNotInProgress = "Нельзя завершить матч, который ещё не начат или уже закончен";
		public const string MatchFinishTimeCannotBeBeforeStart = "Время завершения матча ({0}) не может быть раньше времени начала ({1})";
		public const string TeamNotParticipatingInMatch = "Команда не участвует в этом матче";
		public const string GoalCanOnlyBeAddedToActiveMatch = "Нельзя добавить гол в матч, который сейчас не идёт";
		public const string ScoredTeamNotInMatch = "Нельзя назначить забившей команду, которая не участвует в матче";
		public const string PlayerNotInRoster = "Нельзя назначить автором гола игрока, которого нет в заявке команды";
		public const string InvalidTimeForPeriod = "Указанное время не соответствует текущему периоду";
		public const string GoalNotFoundInMatch = "Гол не найден в событиях этого матча";
		public const string GoalScorerNotInTeamRoster = "Автор гола не заявлен за команду, которая забила гол";
		public const string FirstAssistantNotInTeamRoster = "Первый ассистент не заявлен за команду, которая забила гол";
		public const string SecondAssistantNotInTeamRoster = "Второй ассистент не заявлен за команду, которая забила гол";
		public const string PlayerAlreadyAdded = "Нельзя добавить игрока дважды";
		public const string CannotAddPlayerAfterMatchStart = "Нельзя добавить игрока после начала матча";
		public const string TeamNotInMatch = "Команда не участвует в этом матче";
		public const string MatchNotInProgress = "Матч ещё не начат или уже завершён";
		public const string CannotFinishInfiniteOvertimeWithDraw = "Нельзя завершить бесконечный овертайм при равном счёте";
	}
}
