using SportsStats.Domain.Common;
using SportsStats.Domain.Matches;

namespace SportsStats.Domain.Hockey
{
	public class HockeyGoalEvent : GoalEventBase
	{
		public int? FirstAssistId { get; private set; }
		public int? SecondAssistId { get; private set; }
		public HockeyGoalStrengthType StrengthType { get; private set; }
		public HockeyGoalNetType? NetType { get; private set; }


		internal HockeyGoalEvent(int matchId, int scoringTeamId, int goalScorerId,
								 int period, int time)
			: base(matchId, scoringTeamId, goalScorerId, period, time) { }
		public void SetAssists(int? firstAssistId, int? secondAssistId)
		{
			if (!firstAssistId.HasValue && secondAssistId.HasValue)
				throw new ArgumentException("Второй ассистент не может быть указан, пока не указан первый");

			FirstAssistId = firstAssistId;
			SecondAssistId = secondAssistId;
		}
		public void ChangeFirstAssists(int? assistsId)
		{
			FirstAssistId = assistsId;
		}
		public void ChangeSecondAssists(int? assistsId)
		{
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