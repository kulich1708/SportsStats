using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments
{
	public interface ITournamentRegistrationRepository
	{
		public List<int> FindRegisteredTeamIds(int tournamentId);
		public TournamentRegistration FindById(int registrationId);
		public TournamentRegistration Save(TournamentRegistration tournamentRegistration);
	}
}
