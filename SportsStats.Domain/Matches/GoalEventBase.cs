using SportsStats.Domain.Common;
using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Matches
{
	public abstract class GoalEventBase : BaseEntity
	{
		public int MatchId { get; private set; }
		public int ScoringTeamId { get; private set; }
		public int GoalScorerId { get; private set; }
		public int Period { get; private set; }
		public int Time { get; private set; }

		internal GoalEventBase(int matchId, int scoringTeamId, int goalScorerId, int period, int time)
		{
			MatchId = matchId;
			ScoringTeamId = scoringTeamId;
			GoalScorerId = goalScorerId;
			Period = period;
			Time = time;
		}
	}
}
