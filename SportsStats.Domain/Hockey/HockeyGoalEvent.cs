using SportsStats.Domain.Common;

namespace SportsStats.Domain.Hockey
{
	public class HockeyGoalEvent : BaseEntity
	{
		public int GoalScorerId { get; private set; }
		public int? GoalAssistFirstId { get; private set; }
		public int? GoalAssistSecondId { get; private set; }
		public HockeyGoalNetType? GoalNetType { get; private set; }
		public HockeyGoalStrengthType GoalStrengthType { get; private set; }

		public HockeyGoalEvent(int goalScorerId, HockeyGoalStrengthType goalStrengthType)
		{
			GoalScorerId = goalScorerId;
			GoalStrengthType = goalStrengthType;
		}
		public void SetAssists(int firstAssistId, int secondAssistId)
		{
			GoalAssistFirstId = firstAssistId;
			GoalAssistSecondId = secondAssistId;
		}
		public void SetGoalNetType(HockeyGoalNetType goalNetType)
		{
			GoalNetType = goalNetType;
		}
	}
}