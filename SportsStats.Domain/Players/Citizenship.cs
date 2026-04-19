using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Players
{
	public record Citizenship
	{
		public string Name { get; private set; }
		public byte[]? Photo { get; private set; }
		private Citizenship() { }
		public Citizenship(string name, byte[]? photo = null)
		{
			Name = name;
			Photo = photo;
		}
	}
}
