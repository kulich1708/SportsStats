using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Players
{
	internal class Player<TPosition>
	{
		public string Name { get; private set; }
		public string Surname { get; private set; }
		public DateOnly Birthday { get; private set; }
		public int TeamId { get; private set; }
		public TPosition Position { get; private set; }
		public int JerseyNumber { get; private set; }
		public Player(string name, string surname)
		{
			ValidateNameAndSurname(name, surname);
			Name = name;
			Surname = surname;
		}
		private void ValidateNameAndSurname(string name, string surname)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Имя не может быть пустым");
			if (string.IsNullOrWhiteSpace(surname))
				throw new ArgumentException("Фамилия не может быть пустой");
			if (name.Count() > 20)
				throw new ArgumentException("Имя не может быть длиньше 20 символов");
			if (surname.Count() > 20)
				throw new ArgumentException("Фамилия не может быть длиньше 20 символов");
		}
		public void SetBirthday(DateOnly birthday)
		{
			Birthday = birthday;
		}
		public void SetTeam(int teamId)
		{
			TeamId = teamId;
		}
		public void SetPosition(TPosition position)
		{
			Position = position;
		}
		public void SetJerseyNumber(int jerseyNumber)
		{
			if (jerseyNumber < 1 || jerseyNumber > 99)
				throw new ArgumentException("Номер игрока должен быть от 1 до 99");
			JerseyNumber = jerseyNumber;
		}
	}
}
