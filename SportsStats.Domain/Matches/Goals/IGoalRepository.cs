using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Matches.Goals
{
	public interface IGoalRepository
	{
		public Task<GoalEvent> FindById(int goalId);
		public Task<GoalEvent> Save(GoalEvent goalEvent);
	}
}
