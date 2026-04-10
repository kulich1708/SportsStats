using SportsStats.Domain.Tournaments;
using SportsStats.Domain.Tournaments.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Tournaments.DTOs.Responses
{
	public record TournamentDTO
	(
		string Name,
		DateTime StartedAt,
		DateTime FinishedAt,
		TournamentStatus Status,
		TournamentRules TournamentRules,
		HashSet<int> TeamsId
	);
}
