using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments
{
	public static class TournamentError
	{
		// ===== 2000-2009: Статусы турнира =====
		public static readonly ErrorCode TournamentAlreadyStarted = new(2000, "Турнир уже начат");
		public static readonly ErrorCode TournamentAlreadyFinished = new(2001, "Турнир уже завершён");
		public static readonly ErrorCode TournamentCanOnlyBeFinishedAfterStart = new(2002, "Завершить турнир можно только после его начала");
		public static readonly ErrorCode TournamentRegistrationAlreadyOpen = new(2003, "Регистрация на турнир уже открыта");
		public static readonly ErrorCode RegistrationCanOnlyBeOpenedInDraft = new(2004, "Открыть регистрацию можно только для турнира в статусе Draft");
		public static readonly ErrorCode RegistrationRequiresRules = new(2005, "Для открытия регистрации необходимо установить правила турнира");
		public static readonly ErrorCode TournamentCanOnlyBeStartedAfterRegistration = new(2006, "Турнир можно начать только после завершения регистрации команд");
		public static readonly ErrorCode TournamentRequiresAtLeastTwoTeams = new(2007, "Для начала турнира необходимо минимум 2 команды");

		// ===== 2010-2019: Даты турнира =====
		public static readonly ErrorCode TournamentFinishDateCannotBeBeforeStartDate = new(2010, "Дата завершения турнира ({0}) не может быть раньше даты начала ({1})");
		public static readonly ErrorCode TournamentFinishDateCannotBeBeforeLastMatch = new(2011, "Дата завершения турнира ({0}) не может быть раньше окончания последнего матча ({1})");
		public static readonly ErrorCode TournamentCannotBeFinishedWithUnfinishedMatches = new(2012, "Нельзя завершить турнир: {0} матч(ей) ещё не закончены");

		// ===== 2020-2029: Команды в турнире =====
		public static readonly ErrorCode TeamsCanOnlyBeRegisteredInRegistrationStatus = new(2020, "Заявлять команды можно только на этапе регистрации");
		public static readonly ErrorCode TeamAlreadyRegisteredForTournament = new(2021, "Команда уже заявлена на этот турнир");

		// ===== 2030-2039: Название турнира =====
		public static readonly ErrorCode TournamentNameCannotBeEmpty = new(2030, "Название турнира не может быть пустым");

		// ===== 2040-2049: Правила турнира =====
		public static readonly ErrorCode RulesCanOnlyBeSetForTournamentInDraftStatus = new(2040, "Правила можно установить только для турнира со статусом Draft");
	}
}
