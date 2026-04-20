using SportsStats.Application.Tournaments.DTOs.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Tournaments.DTOs.Responses
{
	public record TournamentDTO
	(
		int Id,
		string Name,
		byte[]? Photo,
		string? PhotoMime,
		DateTime? StartedAt,
		DateTime? FinishedAt,
		string Status,
		TournamentRulesDTO TournamentRules,
		List<TeamInTournamentDTO> Teams
	);
	public record TeamInTournamentDTO(
		int Id,
		string Name
	);
}
