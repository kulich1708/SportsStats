using SportsStats.Domain.Matches;
using System;
using System.Collections.Generic;
using System.Text;
using SportsStats.Domain.Tournaments;

namespace SportsStats.Domain.Services
{
	public interface IMatchCreationService
	{
		public Match CreateMatch(Tournament tournament, int homeTeamId, int awayTeamId);
	}
}
