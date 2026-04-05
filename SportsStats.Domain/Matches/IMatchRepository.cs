using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Matches
{
	public interface IMatchRepository
	{
		public Match FindById(int id);
		public Match Save(Match match);
	}
}
