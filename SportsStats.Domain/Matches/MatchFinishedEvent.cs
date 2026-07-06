using SportsStats.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Matches
{
	public record MatchFinishedEvent(int MatchId) : IDomainEvent;
}
