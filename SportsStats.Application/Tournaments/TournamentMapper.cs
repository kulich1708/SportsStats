using SportsStats.Application.Tournaments.DTOs.Requests;
using SportsStats.Application.Tournaments.DTOs.Responses;
using SportsStats.Domain.Teams;
using SportsStats.Domain.Tournaments;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Tournaments
{
	public static class TournamentMapper
	{
		public static TournamentDTO ToDTO(Tournament tournament, List<Team> teams) => new(
			tournament.Id,
			tournament.Name,
			tournament.StartedAt,
			tournament.FinishedAt,
			tournament.Status.GetDescription(),
			tournament.TournamentRules,
			teams.Select(ToDTO).ToList()
		);
		private static TeamInTournamentDTO ToDTO(Team team) => new(team.Id, team.Name);
	}
}
