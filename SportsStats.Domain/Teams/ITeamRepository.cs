using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Teams
{
	public interface ITeamRepository
	{
		public Team FindById(int teamId);
		public Team Save(Team team);
	}
}
