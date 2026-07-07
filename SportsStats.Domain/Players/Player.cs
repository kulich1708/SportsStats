using SportsStats.Domain.Common;
using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Players
{
	public class Player : BaseEntity, IAggregateRoot
	{
		public string Name { get; private set; }
		public string Surname { get; private set; }
		public int? TeamId { get; private set; }
		public PositionType Position { get; private set; }
		public PlayerNumber? Number { get; private set; }
		public DateOnly? Birthday { get; private set; }
		public Citizenship? Citizenship { get; private set; }
		public byte[]? Photo { get; private set; }
		public string? PhotoMime { get; private set; }
		public Player(string name, string surname, PositionType position)
		{
			SetNameAndSurname(name, surname);
			SetPosition(position);
		}
		public void SetNameAndSurname(string name, string surname)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new DomainException(PlayersError.FirstNameCannotBeEmpty);
			if (string.IsNullOrWhiteSpace(surname))
				throw new DomainException(PlayersError.LastNameCannotBeEmpty);
			if (name.Length > 20)
				throw new DomainException(PlayersError.FirstNameTooLong);
			if (surname.Length > 20)
				throw new DomainException(PlayersError.LastNameTooLong);

			Name = name;
			Surname = surname;
		}
		public void ChangeTeam(int? teamId) => TeamId = teamId;
		public void SetNumber(int? number) => Number = number.HasValue ? new(number.Value) : null;
		public void SetBirthday(DateOnly? birthday) => Birthday = birthday;
		public void SetCitizenship(string? name, byte[]? photo = null, string? photoMime = null)
			=> Citizenship = name == null ? null : new(name, photo, photoMime);
		public void SetPhoto(byte[]? photo, string? photoMime)
		{
			Photo = photo;
			PhotoMime = photoMime;
		}
		public void SetPosition(PositionType position) => Position = position;
	}
}
