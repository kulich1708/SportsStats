using SportsStats.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Players
{
	public class Player : BaseEntity, IAggregateRoot
	{
		public string Name { get; private set; }
		public string Surname { get; private set; }
		public int TeamId { get; private set; }
		public PositionType Position { get; init; }
		public PlayerNumber? Number { get; private set; }
		public DateOnly? Birthday { get; private set; }
		public Citizenship? Citizenship { get; private set; }
		public byte[]? Photo { get; private set; }
		public string? PhotoMime { get; private set; }
		public Player(string name, string surname, PositionType position)
		{
			ValidateNameAndSurname(name, surname);
			Name = name;
			Surname = surname;
			Position = position;
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
		public void ChangeTeam(int teamId) => TeamId = teamId;
		public void SetNumber(int number) => Number = new(number);
		public void SetBirthday(DateOnly birthday) => Birthday = birthday;
		public void SetCitizenship(string name, byte[]? photo = null, string? photoMime = null) => Citizenship = new(name, photo, photoMime);
		public void SetPhoto(byte[] photo, string photoMime)
		{
			Photo = photo;
			PhotoMime = photoMime;
		}
	}
}
