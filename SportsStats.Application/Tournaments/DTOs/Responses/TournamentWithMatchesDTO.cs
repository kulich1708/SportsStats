using SportsStats.Application.Matches.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Tournaments.DTOs.Responses
{
	public record TournamentWithMatchesDTO(
		int Id,
		string Name,
		byte[]? Photo,
		DateTime? StartedAt,
		DateTime? FinishedAt,
		string Status,
		List<MatchShortDTO> Matches
	);
}
