using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Matches.Goals
{
	public interface IGoalRepository
	{
		public GoalEvent FindById(int goalId);
		public GoalEvent Save(GoalEvent goalEvent);
	}
}
