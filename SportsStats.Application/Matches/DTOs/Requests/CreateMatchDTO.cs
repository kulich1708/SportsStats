using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Matches.DTOs.Requests
{
	public record CreateMatchDTO(
		int HomeTeamId,
		int AwayTeamId,
		DateTime ScheduledAt
	);
}
