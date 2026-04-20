using SportsStats.Application.Matches;
using SportsStats.Application.Matches.DTOs.Responses;
using SportsStats.Application.Tournaments.DTOs.Requests;
using SportsStats.Application.Tournaments.DTOs.Responses;
using SportsStats.Application.Tournaments.DTOs.Shared;
using SportsStats.Domain.Matches;
using SportsStats.Domain.Teams;
using SportsStats.Domain.Tournaments;
using SportsStats.Domain.Tournaments.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Tournaments
{
	public static class TournamentMapper
	{
		public static TournamentDTO ToDTO(Tournament tournament, List<Team> teams)
		{
			Console.WriteLine(tournament.TournamentRules == null);
			return new(
			tournament.Id,
			tournament.Name,
			tournament.Photo,
			tournament.PhotoMime,
			tournament.StartedAt,
			tournament.FinishedAt,
			tournament.Status.GetDescription(),
			tournament.TournamentRules == null ? null : ToDTO(tournament.TournamentRules),
			teams.Select(ToDTO).ToList()
		);
		}
		private static TeamInTournamentDTO ToDTO(Team team) => new(team.Id, team.Name);

		public static TournamentShortDTO ToDTO(Tournament tournament) => new(
			tournament.Id,
			tournament.Name,
			tournament.Photo,
			tournament.PhotoMime,
			tournament.Status.GetDescription()
		);
		public static TournamentWithMatchesDTO ToDTO(Tournament tournament, List<MatchShortDTO> matches) => new(
			tournament.Id,
			tournament.Name,
			tournament.Photo,
			tournament.PhotoMime,
			tournament.StartedAt,
			tournament.FinishedAt,
			tournament.Status.GetDescription(),
			matches
		);
		private static MatchPointsRulesDTO ToDTO(MatchPointsRules rules) => new(
			rules.WinPoints,
			rules.OTWinPoints,
			rules.LossPoints,
			rules.OTLossPoints,
			rules.DrawPoints
		);
		private static MatchRosterRulesDTO ToDTO(MatchRosterRules rules) => new(
			rules.MaxPlayers,
			rules.MinPlayers,
			rules.MinForwards,
			rules.MaxForwards,
			rules.MinDefensemans,
			rules.MaxDefensemans,
			rules.MinGoalies,
			rules.MaxGoalies
		);
		private static MatchDurationRulesDTO ToDTO(MatchDurationRules rules) => new(
			rules.PeriodsCount,
			rules.PeriodDurationSeconds,
			rules.HasOvertime,
			rules.OvertimeDurationSeconds,
			rules.OvertimesCount,
			rules.SuddenDeathOvertime,
			rules.IsDrawPossible,
			rules.ShootoutsCount
		);
		public static TournamentRulesDTO ToDTO(TournamentRules rules) => new(
			rules.MatchDurationRules == null ? null : ToDTO(rules.MatchDurationRules),
			rules.MatchRosterRules == null ? null : ToDTO(rules.MatchRosterRules),
			rules.MatchPointsRules == null ? null : ToDTO(rules.MatchPointsRules)
		);
		private static MatchPointsRules ToDomain(MatchPointsRulesDTO rules) => new(
			rules.WinPoints,
			rules.OTWinPoints,
			rules.LossPoints,
			rules.OTLossPoints,
			rules.DrawPoints
		);
		private static MatchRosterRules ToDomain(MatchRosterRulesDTO rules) => new(
			rules.MaxPlayers,
			rules.MinPlayers,
			rules.MinForwards,
			rules.MaxForwards,
			rules.MinDefensemans,
			rules.MaxDefensemans,
			rules.MinGoalies,
			rules.MaxGoalies
		);

		private static MatchDurationRules ToDomain(MatchDurationRulesDTO rules) => new(
			rules.PeriodsCount,
			rules.PeriodDurationSeconds,
			rules.HasOvertime,
			rules.OvertimeDurationSeconds,
			rules.OvertimesCount,
			rules.SuddenDeathOvertime,
			rules.IsDrawPossible,
			rules.ShootoutsCount
		);
		public static TournamentRules ToDomain(TournamentRulesDTO rules) => new(
			ToDomain(rules.MatchDurationRules),
			ToDomain(rules.MatchRosterRules),
			ToDomain(rules.MatchPointsRules)
		);
	}
}
