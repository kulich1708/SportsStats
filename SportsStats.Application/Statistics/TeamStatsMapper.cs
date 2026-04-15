using SportsStats.Application.Statistics.DTOs.Responses;
using SportsStats.Domain.Statistics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Statistics
{
	public static class TeamStatsMapper
	{
		public static TeamStatsDTO ToDTO(TeamStats stats) => new(
			stats.TeamId,
			stats.TournamentId,
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
