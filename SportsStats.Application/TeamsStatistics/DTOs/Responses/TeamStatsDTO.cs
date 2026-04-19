using SportsStats.Domain.Teams;
using SportsStats.Domain.Tournaments;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Statistics.DTOs.Responses
{
	public record TeamStatsDTO(
		int TeamId,
		string TeamName,
		int TournamentId,
		string TournamentName,
		int Games,
		int RegularWins,
		int OTWins,
		int Draws,
		int OTLosses,
		int RegularLosses,
		int Points
	)
	{ }
}
