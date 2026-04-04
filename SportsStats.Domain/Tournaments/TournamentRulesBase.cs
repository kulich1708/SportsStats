using SportsStats.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments
{
	public class TournamentRulesBase : BaseEntity
	{
		public string Name { get; private set; }
		public TournamentRulesBase(string name)
		{
			Name = name;
		}
	}
}
