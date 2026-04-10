using SportsStats.Application.Matches.DTOs.Responses;
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
		public static MatchDTO ToDTO(Match match) => new(
			match.HomeTeamId,
			match.AwayTeamId,
			match.HomeTeamRoster.ToList(),
			match.AwayTeamRoster.ToList(),
			match.StartedAt,
			match.FinishedAt,
			match.TournamentId,
			match.Status,
			match.HomeTeamScore,
			match.AwayTeamScore,
			match.HomeTeamWinType,
			match.AwayTeamWinType,
			match.IsOvertime,
			match.Goals.Select(MatchMapper.ToDTO).ToList(),
			match.Rules
		);

		public static GoalDTO ToDTO(GoalEvent goal) => new(
			goal.ScoringTeamId,
			goal.GoalScorerId,
			goal.Period,
			goal.Time,
			goal.FirstAssistId,
			goal.SecondAssistId,
			goal.StrengthType,
			goal.NetType
			);
	}
}
