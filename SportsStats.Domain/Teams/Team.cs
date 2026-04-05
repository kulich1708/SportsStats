using SportsStats.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Teams
{
	public class Team : BaseEntity, IAggregateRoot
	{
		public string Name { get; private set; }
	}
}
