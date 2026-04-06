using SportsStats.Domain.Common;
using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Matches.Goals
{
	public class GoalEvent : BaseEntity
	{
		public int ScoringTeamId { get; private set; }
		public int GoalScorerId { get; private set; }
		public int Period { get; private set; }
		public int Time { get; private set; }
		public int? FirstAssistId { get; private set; }
		public int? SecondAssistId { get; private set; }
		public GoalStrengthType? StrengthType { get; private set; }
		public GoalNetType? NetType { get; private set; }


		public GoalEvent(int scoringTeamId, int goalScorerId, int period, int time)
		{
			ScoringTeamId = scoringTeamId;
			GoalScorerId = goalScorerId;
			Period = period;
			Time = time;
		}


		public void SetAssists(int? firstAssistId, int? secondAssistId)
		{
			if (firstAssistId.HasValue && secondAssistId.HasValue &&
				firstAssistId.Value == secondAssistId.Value)
				throw new ArgumentException("Нельзя установить первым и вторым ассистентом одного и того же игрока");
			if (firstAssistId == GoalScorerId || secondAssistId == GoalScorerId)
				throw new ArgumentException("Нельзя установить ассистентом игрока, который забил этот гол");
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
		public void SetNetType(GoalNetType? netType)
		{
			NetType = netType;
		}
		public void SetStrengthType(GoalStrengthType strengthType)
		{
			StrengthType = strengthType;
		}
	}
}
