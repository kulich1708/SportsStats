using SportsStats.Application.Teams.DTOs.Responses;
using SportsStats.Domain.Matches;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Matches.DTOs.Responses
{
	public record MatchShortDTO(
		int Id,
		int TournamentId,
		TeamDTO HomeTeam,
		TeamDTO AwayTeam,
		DateTime ScheduleAt,
		DateTime? StartedAt,
		DateTime? FinishedAt,
		MatchStatusDTO Status,
		int HomeTeamScore,
		int AwayTeamScore,
		string HomeTeamWinType,
		string AwayTeamWinType,
		bool IsOvertime
	);
}
