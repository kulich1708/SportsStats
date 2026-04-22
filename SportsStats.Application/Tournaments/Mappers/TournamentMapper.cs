using SportsStats.Application.Matches.DTOs.Responses;
using SportsStats.Application.Tournaments.DTOs.Responses;
using SportsStats.Application.Tournaments.Mappers.Rules;
using SportsStats.Domain.Matches;
using SportsStats.Domain.Teams;
using SportsStats.Domain.Tournaments;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SportsStats.Application.Tournaments.Mappers
{
	public static class TournamentMapper
	{
		public static TournamentDTO ToDTO(Tournament tournament, List<Team> teams)
		{
			return new(
			tournament.Id,
			tournament.Name,
			tournament.Photo,
			tournament.PhotoMime,
			tournament.StartedAt,
			tournament.FinishedAt,
			ToDTO(tournament.Status),
			tournament.TournamentRules == null ? null : MatchRulesMapper.ToDTO(tournament.TournamentRules),
			teams.Select(ToDTO).ToList()
		);
		}
		private static TeamInTournamentDTO ToDTO(Team team) => new(team.Id, team.Name);

		public static TournamentShortDTO ToDTO(Tournament tournament) => new(
			tournament.Id,
			tournament.Name,
			tournament.Photo,
			tournament.PhotoMime,
			ToDTO(tournament.Status)
		);
		public static TournamentWithMatchesDTO ToDTO(Tournament tournament, List<MatchShortDTO> matches) => new(
			tournament.Id,
			tournament.Name,
			tournament.Photo,
			tournament.PhotoMime,
			tournament.StartedAt,
			tournament.FinishedAt,
			ToDTO(tournament.Status),
			matches
		);

		public static TournamentStatusDTO ToDTO(TournamentStatus status) => new(
			status,
			status.GetDescription(),
			status.GetNextActionStatusDescription()
		);
	}
}
