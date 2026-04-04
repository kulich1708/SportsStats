using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Shared
{
	internal interface IExistenceChecker
	{
		public bool Exists(int matchId);
	}
}
