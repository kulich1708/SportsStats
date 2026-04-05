using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments
{
	public interface ITournamentRepository
	{
		public Tournament FindById(int tournamentId);
		public Tournament Save(Tournament tournament);
	}
}
