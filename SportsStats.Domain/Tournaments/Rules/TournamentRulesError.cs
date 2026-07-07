using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments.Rules
{
	public static class TournamentRulesError
	{
		public const string MatchTimeRulesRequired = "Необходимо указать правила длины матча";
		public const string MatchRosterRulesRequired = "Необходимо указать правила заявки игроков на матч";
		public const string MatchPointsRulesRequired = "Необходимо указать правила начисления очков";
	}
}
