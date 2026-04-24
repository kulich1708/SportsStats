using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Tournaments.DTOs.Responses
{
	public record TournamentShortDTO(
		int Id,
		string Name,
		byte[]? Photo,
		string? PhotoMime,
		TournamentStatusDTO Status
	);
}
