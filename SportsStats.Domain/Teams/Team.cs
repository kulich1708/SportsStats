using SportsStats.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Teams
{
	public class Team : BaseEntity, IAggregateRoot
	{
		public string Name { get; private set; }
		public string? City { get; private set; }
		public byte[]? Photo { get; private set; }
		public string? PhotoMime { get; private set; }
		public Team(string name, string? city = null)
		{
			SetName(name);
			SetCity(city);
		}

		public void SetPhoto(byte[] photo, string photoMime)
		{
			Photo = photo;
			PhotoMime = photoMime;
		}
		public void SetName(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Имя команды не может быть пустным");

			Name = name;
		}
		public void SetCity(string? city)
		{
			City = city;
		}
	}
}
