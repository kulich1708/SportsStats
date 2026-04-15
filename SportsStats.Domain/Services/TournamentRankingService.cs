using SportsStats.Domain.Statistics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Services
{
	public class TournamentRankingService
	{
		public List<TeamStats> RankingByPoints(List<TeamStats> teamsStats)
		{
			return teamsStats.OrderBy(stat => stat.Points)
							 .ThenBy(stat => stat.RegularWins)
							 .ThenBy(stat => stat.OTWins)
							 .ThenBy(stat => stat.Draws)
							 .ToList();
		}
	}
}
