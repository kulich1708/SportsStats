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

		private byte[]? _photo;
		public IReadOnlyCollection<byte>? Photo => _photo?.AsReadOnly();
		public Team(string name, string? city = null)
		{
			Name = name;
			City = city;
		}

		public void SetPhoto(byte[] photo) => _photo = photo;
	}
}
