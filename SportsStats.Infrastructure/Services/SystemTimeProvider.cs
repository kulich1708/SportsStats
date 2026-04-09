using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Infrastructure.Services
{
	public class SystemTimeProvider : ITimeProvider
	{
		public DateTime GetCurrentTime()
		{
			return DateTime.UtcNow;
		}
	}
}
