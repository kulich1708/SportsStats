using SportsStats.Application.Matches.DTOs.Responses;
using SportsStats.Application.Players.DTOs.Responses;
using SportsStats.Application.Teams.DTOs.Responses;
using SportsStats.Application.Tournaments.DTOs.Responses;
using SportsStats.Application.Tournaments;
using SportsStats.Domain.Matches;
using SportsStats.Domain.Matches.Goals;
using SportsStats.Domain.Shared.Enums;
using SportsStats.Domain.Tournaments;
using SportsStats.Domain.Tournaments.Rules;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.NetworkInformation;
using System.Text;
using System.Timers;

namespace SportsStats.Application.Matches
{
	public static class MatchMapper
	{
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
				match.Status.GetDescription(),
				match.HomeTeamScore,
				match.AwayTeamScore,
				match.HomeTeamWinType.GetDescription(),
				match.AwayTeamWinType.GetDescription(),
				match.IsOvertime,
				match.Goals.Select(
					g => MatchMapper.ToDTO(g, g.ScoringTeamId == match.HomeTeamId ? homeTeam : awayTeam,
											g.ScoringTeamId == match.HomeTeamId ? homeTeamRoster : awayTeamRoster))
					.ToList(),
				TournamentMapper.ToDTO(match.Rules)
			);

		public static GoalDTO ToDTO(
			GoalEvent goal,
			TeamDTO scoringTeam,
			List<PlayerDTO> teamRoster
			) => new(
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
			match.Status.GetDescription(),
			match.HomeTeamScore,
			match.AwayTeamScore,
			match.HomeTeamWinType.GetDescription(),
			match.AwayTeamWinType.GetDescription(),
			match.IsOvertime
		);
	}
}
