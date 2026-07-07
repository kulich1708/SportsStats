using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Matches.Goals
{
	public class GoalsError
	{
		public const string AssistantsMustBeDifferent = "Первый и второй ассистенты не могут быть одним и тем же игроком";
		public const string AssistantCannotBeGoalScorer = "Ассистентом не может быть игрок, забивший гол";
		public const string SecondAssistantRequiresFirst = "Нельзя указать второго ассистента без первого";
	}
}
