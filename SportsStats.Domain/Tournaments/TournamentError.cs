using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments
{
	public static class TournamentError
	{
		public const string RulesCanOnlyBeSetForTournamentInDraftStatus = "Правила можно установить только для турнира со статусом Draft";
		public const string TournamentAlreadyStarted = "Турнир уже начат";
		public const string TournamentCanOnlyBeStartedAfterRegistration = "Турнир можно начать только после завершения регистрации команд";
		public const string TournamentRequiresAtLeastTwoTeams = "Для начала турнира необходимо минимум 2 команды";
		public const string TournamentAlreadyFinished = "Турнир уже завершён";
		public const string TournamentCanOnlyBeFinishedAfterStart = "Завершить турнир можно только после его начала";
		public const string TournamentFinishDateCannotBeBeforeStartDate = "Дата завершения турнира ({0}) не может быть раньше даты начала ({1})";
		public const string TournamentCannotBeFinishedWithUnfinishedMatches = "Нельзя завершить турнир: {0} матч(ей) ещё не закончены";
		public const string TournamentFinishDateCannotBeBeforeLastMatch = "Дата завершения турнира ({0}) не может быть раньше окончания последнего матча ({1})";
		public const string TournamentRegistrationAlreadyOpen = "Регистрация на турнир уже открыта";
		public const string RegistrationCanOnlyBeOpenedInDraft = "Открыть регистрацию можно только для турнира в статусе Draft";
		public const string RegistrationRequiresRules = "Для открытия регистрации необходимо установить правила турнира";
		public const string TeamsCanOnlyBeRegisteredInRegistrationStatus = "Заявлять команды можно только на этапе регистрации";
		public const string TeamAlreadyRegisteredForTournament = "Команда уже заявлена на этот турнир";
		public const string TournamentNameCannotBeEmpty = "Название турнира не может быть пустым";
	}
}
