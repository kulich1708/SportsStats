using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Matches
{
	public interface IMatchRepository
	{
		public Task<Match?> FindById(int id);
		public Task<Match> Save(Match match);
	}
}
