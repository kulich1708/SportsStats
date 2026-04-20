using SportsStats.Domain.Matches;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Matches.DTOs.Responses
{
	public record MatchStatusDTO(
		MatchStatus Code,
		string Description,
		string NextActionDescription
	);
}
