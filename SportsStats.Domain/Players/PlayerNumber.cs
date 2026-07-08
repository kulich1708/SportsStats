using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Players
{
	public record PlayerNumber
	{
		public int Number { get; private set; }
		public PlayerNumber(int number)
		{
			Number = number;
			Validate();
		}
		private void Validate()
		{
			if (Number < 1 || Number > 99)
				throw new DomainException(PlayerError.PlayerNumberOutOfRange);
		}
	}
}
