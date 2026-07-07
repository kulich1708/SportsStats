using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Matches
{
	public record Period(int Current, bool IsBreak, string? Title)
	{
		public void ValidateFinish()
		{
			if (IsBreak)
				throw new DomainException(MatchError.PeriodAlreadyFinished);
		}
		public void ValidateStart()
		{
			if (!IsBreak)
				throw new DomainException(MatchError.PeriodAlreadyStarted);
		}
	}
}
