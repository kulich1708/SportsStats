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
			Validate(number);
			Number = number;
		}
		private void Validate(int number)
		{
			if (number < 1 || number > 99)
				throw new ArgumentException("Номер игрока должен быть от 1 до 99");
		}
	}
}
