using SportsStats.Domain.Matches.Goals;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Matches.DTOs.Responses
{
	public record GoalDTO(
		int ScoringTeamId,
		int GoalScorerId,
		int Period,
		int Time,
		int? FirstAssistId,
		int? SecondAssistId,
		GoalStrengthType? StrengthType,
		GoalNetType? NetType
		);
}
