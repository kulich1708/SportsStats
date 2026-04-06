using SportsStats.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Players
{
	public class Player : IAggregateRoot
	{
		public string Name { get; private set; }
		public string Surname { get; private set; }
		public DateOnly Birthday { get; private set; }
		public int TeamId { get; private set; }
		public PositionType Position { get; private set; }
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
				throw new ArgumentException("Имя не может быть длинее 20 символов");
			if (surname.Count() > 20)
				throw new ArgumentException("Фамилия не может быть длинее 20 символов");
		}
		public void SetBirthday(DateOnly birthday)
		{
			Birthday = birthday;
		}
		public void ChangeTeam(int teamId)
		{
			TeamId = teamId;
		}
		public void SetPosition(PositionType position)
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
