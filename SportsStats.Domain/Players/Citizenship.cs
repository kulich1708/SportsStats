using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Players
{
	public record Citizenship
	{
		public string Name { get; private set; }
		private byte[]? _photo;
		public IReadOnlyCollection<byte>? Photo => _photo?.AsReadOnly();
		public Citizenship(string name, byte[]? photo = null)
		{
			Name = name;
			_photo = photo;
		}
	}
}
