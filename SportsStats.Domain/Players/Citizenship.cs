using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Players
{
	public record Citizenship
	{
		public string Name { get; private set; }
		public byte[]? Photo { get; private set; }
		public string? PhotoMime { get; private set; }
		private Citizenship() { }
		public Citizenship(string name, byte[]? photo = null, string? photoMime = null)
		{
			Console.WriteLine("Пришло в Cit " + name);
			Name = name;
			Photo = photo;
			PhotoMime = photoMime;
		}
	}
}
