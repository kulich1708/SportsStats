using SportsStats.Domain.Matches;
using SportsStats.Domain.Shared.Enums;
using SportsStats.Domain.Tournaments.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Matches.DTOs.Responses
{
	public record MatchDTO(
		int HomeTeamId,
		int AwayTeamId,
		HashSet<int> HomeTeamRoster,
		HashSet<int> AwayTeamRoster,
		DateTime? StartedAt,
		DateTime? FinishedAt,
		int TournamentId,
		MatchStatus Status,
		int HomeTeamScore,
		int AwayTeamScore,
		MatchWinType? HomeTeamWinType,
		MatchWinType? AwayTeamWinType,
		bool IsOvertime,
		List<GoalDTO> Goals,
		TournamentRules Rules
		);
}
