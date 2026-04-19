using SportsStats.Application.Statistics.DTOs.Responses;
using SportsStats.Domain.Statistics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Statistics
{
	public static class TeamStatsMapper
	{
		public static TeamStatsDTO ToDTO(TeamStats stats, string teamName, string tournamentName) => new(
			stats.TeamId,
			teamName,
			stats.TournamentId,
			tournamentName,
			stats.Games,
			stats.RegularWins,
			stats.OTWins,
			stats.Draws,
			stats.OTLosses,
			stats.RegularLosses,
			stats.Points
		);
	}
}
