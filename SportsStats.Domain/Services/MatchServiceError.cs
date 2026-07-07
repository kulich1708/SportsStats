using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Services
{
	public class MatchServiceError
	{
		public const string MatchCanOnlyBeCreatedInRegistrationOrStarted = "Нельзя создать матч в турнире, который ещё не открыт";
		public const string MatchScheduleTimeCannotBeBeforeTournamentStart = "Время матча ({0}) не может быть раньше начала турнира ({1})";
		public const string HomeTeamNotRegistered = "Домашняя команда не заявлена на турнир";
		public const string AwayTeamNotRegistered = "Гостевая команда не заявлена на турнир";
		public const string MatchCannotBeStartedInNotStartedTournament = "Нельзя начать матч в неначатом турнире";
		public const string MatchCannotBeStartedBeforeTournamentStart = "Время начала матча ({0}) не может быть раньше начала турнира ({1})";
		public const string ForwardsCountOutOfRange = "Количество нападающих в команде {0} должно быть от {1} до {2}";
		public const string DefensemenCountOutOfRange = "Количество защитников в команде {0} должно быть от {1} до {2}";
		public const string GoaliesCountOutOfRange = "Количество вратарей в команде {0} должно быть от {1} до {2}";
		public const string PlayersCountOutOfRange = "Количество игроков в команде {0} должно быть от {1} до {2}";
	}
}
