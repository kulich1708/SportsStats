using SportsStats.Domain.Common;
using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Matches.Goals
{
	public class GoalEvent : BaseEntity
	{
		public int MatchId { get; private set; }
		public int ScoringTeamId { get; private set; }
		public int GoalScorerId { get; private set; }
		public int Period { get; private set; }
		public int Time { get; private set; }
		public int? FirstAssistId { get; private set; }
		public int? SecondAssistId { get; private set; }
		public HockeyGoalStrengthType StrengthType { get; private set; }
		public HockeyGoalNetType? NetType { get; private set; }


		public GoalEvent(int matchId, int scoringTeamId, int goalScorerId, int period, int time)
		{
			MatchId = matchId;
			ScoringTeamId = scoringTeamId;
			GoalScorerId = goalScorerId;
			Period = period;
			Time = time;
		}


		public void SetAssists(int? firstAssistId, int? secondAssistId)
		{
			SetFirstAssists(firstAssistId);
			SetSecondAssists(secondAssistId);
		}
		public void SetFirstAssists(int? assistsId)
		{
			FirstAssistId = assistsId;
		}
		public void SetSecondAssists(int? assistsId)
		{
			if (!FirstAssistId.HasValue && assistsId.HasValue)
				throw new ArgumentException("Второй ассистент не может быть указан, пока не указан первый");

			SecondAssistId = assistsId;
		}
		public void SetNetType(HockeyGoalNetType? netType)
		{
			NetType = netType;
		}
		public void SetStrengthType(HockeyGoalStrengthType strengthType)
		{
			StrengthType = strengthType;
		}
	}
}
