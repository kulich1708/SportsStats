using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Matches.DTOs.Requests
{
	public record AddGoalDTO(
		int ScoringTeamId,
		int GoalScorerId,
		int Period,
		int Time
		);
}
