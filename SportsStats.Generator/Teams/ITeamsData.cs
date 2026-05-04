using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp.Teams
{
	public interface ITeamsData
	{
		public List<(string, string, int)> Data { get; }
	}
}
