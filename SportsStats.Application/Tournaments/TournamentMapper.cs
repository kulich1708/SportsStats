using SportsStats.Application.Tournaments.DTOs.Requests;
using SportsStats.Application.Tournaments.DTOs.Responses;
using SportsStats.Domain.Tournaments;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Tournaments
{
	public static class TournamentMapper
	{
		public static TournamentDTO ToDTO(Tournament tournament) => new(
				tournament.Name,
				tournament.StartedAt,
				tournament.FinishedAt,
				tournament.Status,
				tournament.TournamentRules,
				tournament.TeamsId.ToHashSet()
			);
	}
}
