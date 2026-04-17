using SportsStats.Application.Teams.DTOs.Responses;
using SportsStats.Domain.Matches;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Matches.DTOs.Responses
{
	public record MatchShortDTO(
		int Id,
		TeamDTO HomeTeam,
		TeamDTO AwayTeam,
		DateTime? StartedAt,
		DateTime? FinishedAt,
		string Status,
		int HomeTeamScore,
		int AwayTeamScore,
		string HomeTeamWinType,
		string AwayTeamWinType,
		bool IsOvertime
	);
}
