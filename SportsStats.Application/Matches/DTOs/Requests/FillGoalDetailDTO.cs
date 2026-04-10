using SportsStats.Domain.Matches.Goals;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Matches.DTOs.Requests
{
	public record FillGoalDetailDTO(
		int? FirstAssistId,
		int? SecondAssistId,
		GoalStrengthType StrengthType,
		GoalNetType? NetType
		);
}
