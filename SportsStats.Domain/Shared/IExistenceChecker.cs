using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Shared
{
	public interface IExistenceChecker
	{
		public bool Exists(int matchId);
	}
}
