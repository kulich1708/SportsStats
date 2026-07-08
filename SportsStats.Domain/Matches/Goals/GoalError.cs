using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Matches.Goals
{
	public static class GoalError
	{
		// ===== 1100-1109: Ассистенты =====
		public static readonly ErrorCode AssistantsMustBeDifferent = new(1100, "Первый и второй ассистенты не могут быть одним и тем же игроком");
		public static readonly ErrorCode AssistantCannotBeGoalScorer = new(1101, "Ассистентом не может быть игрок, забивший гол");
		public static readonly ErrorCode SecondAssistantRequiresFirst = new(1102, "Нельзя указать второго ассистента без первого");
	}
}
