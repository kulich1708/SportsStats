using SportsStats.Application.Players.DTOs.Responses;
using SportsStats.Application.Teams.DTOs.Responses;
using SportsStats.Application.Tournaments.DTOs.Responses;
using SportsStats.Domain.Matches;
using SportsStats.Domain.Shared.Enums;
using SportsStats.Domain.Tournaments.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Matches.DTOs.Responses
{
	public record MatchDTO(
		TeamDTO HomeTeam,
		TeamDTO AwayTeam,
		List<PlayerDTO> HomeTeamRoster,
		List<PlayerDTO> AwayTeamRoster,
		DateTime? StartedAt,
		DateTime? FinishedAt,
		int TournamentId,
		string TournamentName,
		string Status,
		int HomeTeamScore,
		int AwayTeamScore,
		string HomeTeamWinType,
		string AwayTeamWinType,
		bool IsOvertime,
		List<GoalDTO> Goals,
		TournamentRules Rules
	);
}
