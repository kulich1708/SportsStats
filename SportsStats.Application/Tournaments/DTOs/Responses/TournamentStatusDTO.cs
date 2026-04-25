using SportsStats.Domain.Tournaments;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Tournaments.DTOs.Responses
{
	public record TournamentStatusDTO(
		TournamentStatus Code,
		string Description,
		string NextActionDescription
	);
}
