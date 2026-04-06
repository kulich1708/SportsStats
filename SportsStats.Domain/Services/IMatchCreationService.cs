using SportsStats.Domain.Matches;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Services
{
	public interface IMatchCreationService
	{
		public Match CreateMatch(int tournamentId, int homeTeamId, int awayTeamId);
	}
}
