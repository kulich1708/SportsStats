using SportsStats.Application.Matches.DTOs.Responses;
using SportsStats.Application.Players.DTOs.Responses;
using SportsStats.Application.Teams.DTOs.Responses;
using SportsStats.Application.Tournaments.DTOs.Responses;
using SportsStats.Domain.Matches;
using SportsStats.Domain.Matches.Goals;
using SportsStats.Domain.Shared.Enums;
using SportsStats.Domain.Tournaments;
using SportsStats.Application.Tournaments.Mappers.Rules;
using System.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Matches
{
	public static class MatchMapper
	{
		public static MatchStatusDTO ToDTO(MatchStatus status)
			=> new(status, status.GetDescription(), status.GetNextActionDescription());
		public static MatchDTO ToDTO(
			Match match,
			TeamDTO homeTeam,
			TeamDTO awayTeam,
			List<PlayerDTO> homeTeamRoster,
			List<PlayerDTO> awayTeamRoster,
			TournamentShortDTO tournament) => new(
				match.Id,
				homeTeam,
				awayTeam,
				homeTeamRoster,
				awayTeamRoster,
				match.ScheduledAt,
				match.StartedAt,
				match.FinishedAt,
				tournament,
				ToDTO(match.Status),
				match.HomeTeam.Score,
				match.AwayTeam.Score,
				match.HomeTeam.WinType.GetDescription(),
				match.AwayTeam.WinType.GetDescription(),
				match.IsOvertime,
				ToDTO(match.Period),
				match.Goals.Select(
					g => MatchMapper.ToDTO(g, g.ScoringTeamId == match.HomeTeam.Id ? homeTeam : awayTeam,
											g.ScoringTeamId == match.HomeTeam.Id ? homeTeamRoster : awayTeamRoster))
					.ToList(),
				MatchRulesMapper.ToDTO(match.Rules)
			);

		public static GoalDTO ToDTO(
			GoalEvent goal,
			TeamDTO scoringTeam,
			List<PlayerDTO> teamRoster
			) => new(
				goal.Id,
				scoringTeam,
				teamRoster.First(p => p.Id == goal.GoalScorerId),
				goal.Period,
				goal.Time,
				teamRoster.FirstOrDefault(p => p.Id == goal.FirstAssistId),
				teamRoster.FirstOrDefault(p => p.Id == goal.SecondAssistId),
				goal.StrengthType?.GetDescription() ?? string.Empty,
				goal.NetType?.GetDescription() ?? string.Empty
			);
		public static MatchShortDTO ToDTO(Match match, TeamDTO homeTeam, TeamDTO awayTeam) => new(
			match.Id,
			match.TournamentId,
			homeTeam,
			awayTeam,
			match.ScheduledAt,
			match.StartedAt,
			match.FinishedAt,
			ToDTO(match.Status),
			match.HomeTeam.Score,
			match.AwayTeam.Score,
			match.HomeTeam.WinType.GetDescription(),
			match.AwayTeam.WinType.GetDescription(),
			match.IsOvertime,
			ToShortPeriodDTO(match.Period)
		);
		public static PeriodDTO ToDTO(Period period) => new(period.Current, period.IsBreak, period.Title);
		public static PeriodShortDTO ToShortPeriodDTO(Period period) => new(period.Current, period.IsBreak);
	}
}
