using SportsStats.Application.Players.DTOs.Responses;
using SportsStats.Application.Teams.DTOs.Responses;
using SportsStats.Domain.Matches.Goals;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Matches.DTOs.Responses
{
	public record GoalDTO(
		int Id,
		TeamDTO ScoringTeamId,
		PlayerDTO GoalScorerId,
		int Period,
		int Time,
		PlayerDTO? FirstAssistId,
		PlayerDTO? SecondAssistId,
		string StrengthType,
		string NetType
		);
}
