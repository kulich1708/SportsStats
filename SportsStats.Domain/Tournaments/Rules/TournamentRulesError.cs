using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments.Rules
{
	public static class TournamentRulesError
	{
		// ===== 3000-3009: Общие правила турнира =====
		public static readonly ErrorCode MatchTimeRulesRequired = new(3000, "Необходимо указать правила длины матча");
		public static readonly ErrorCode MatchRosterRulesRequired = new(3001, "Необходимо указать правила заявки игроков на матч");
		public static readonly ErrorCode MatchPointsRulesRequired = new(3002, "Необходимо указать правила начисления очков");
	}
}
