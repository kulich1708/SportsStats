using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Players
{
	public class PlayersError
	{
		public const string PlayerNumberOutOfRange = "Номер игрока должен быть в диапазоне от 1 до 99";
		public const string FirstNameCannotBeEmpty = "Имя не может быть пустым";
		public const string LastNameCannotBeEmpty = "Фамилия не может быть пустой";
		public const string FirstNameTooLong = "Имя не может превышать 20 символов";
		public const string LastNameTooLong = "Фамилия не может превышать 20 символов";
	}
}
