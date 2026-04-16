using SportsStats.Application.Tournaments.DTOs.Requests;
using SportsStats.Application.Tournaments.DTOs.Responses;
using SportsStats.Application.Tournaments.DTOs.Shared;
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
		public static TournamentDTO ToDTO(Tournament tournament, List<Team> teams) => new(
			tournament.Id,
			tournament.Name,
			tournament.StartedAt,
			tournament.FinishedAt,
			tournament.Status.GetDescription(),
			ToDTO(tournament.TournamentRules),
			teams.Select(ToDTO).ToList()
		);
		private static TeamInTournamentDTO ToDTO(Team team) => new(team.Id, team.Name);
		public static MatchPointsRulesDTO ToDTO(MatchPointsRules rules) => new(
			rules.WinPoints,
			rules.OTWinPoints,
			rules.LossPoints,
			rules.OTLossPoints,
			rules.DrawPoints
		);
		public static MatchRosterRulesDTO ToDTO(MatchRosterRules rules) => new(
			rules.MaxPlayers,
			rules.MinPlayers,
			rules.MinForwards,
			rules.MaxForwards,
			rules.MinDefensemans,
			rules.MaxDefensemans,
			rules.MinGoalies,
			rules.MaxGoalies
		);
		public static MatchDurationRulesDTO ToDTO(MatchDurationRules rules) => new(
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
			ToDTO(rules.MatchDurationRules),
			ToDTO(rules.MatchRosterRules),
			ToDTO(rules.MatchPointsRules)
		);
	}
}
