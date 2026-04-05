using SportsStats.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments.Rules
{
	public class MatchRosterRules
	{
		public int MaxPlayers { get; private set; }
		public int MinPlayers { get; private set; }
		public int MinForwards { get; private set; }
		public int MaxForwards { get; private set; }
		public int MinDefensemans { get; private set; }
		public int MaxDefensemans { get; private set; }
		public int MinGoalies { get; private set; }
		public int MaxGoalies { get; private set; }

		public MatchRosterRules(int maxPlayers = 20, int minPlayers = 20,
			int minForwards = 12, int maxForwards = 12,
			int minDefensemans = 6, int maxDefensemans = 6,
			int minGoalies = 2, int maxGoalies = 2)
		{
			MaxPlayers = maxPlayers;
			MinPlayers = minPlayers;
			MinForwards = minForwards;
			MaxForwards = maxForwards;
			MinDefensemans = minDefensemans;
			MaxDefensemans = maxDefensemans;
			MinGoalies = minGoalies;
			MaxGoalies = maxGoalies;

			ValidateRoster();
		}
		private void ValidateRoster()
		{
			int calculatedMinPlayers = MinForwards + MinDefensemans + MinGoalies;
			int calculatedMaxPlayers = MaxForwards + MaxDefensemans + MaxGoalies;

			if (MaxPlayers < calculatedMinPlayers || MinPlayers > calculatedMaxPlayers)
				throw new ArgumentException("Проверьте настройки состава");
		}
	}
}
