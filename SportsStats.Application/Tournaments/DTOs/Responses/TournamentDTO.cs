using SportsStats.Domain.Tournaments;
using SportsStats.Domain.Tournaments.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Tournaments.DTOs.Responses
{
	public record TournamentDTO
	(
		int Id,
		string Name,
		DateTime StartedAt,
		DateTime FinishedAt,
		string Status,
		TournamentRules TournamentRules,
		List<TeamInTournamentDTO> Teams
	);
	public record TeamInTournamentDTO(
		int Id,
		string Name
	);
}
