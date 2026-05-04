using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp.Players
{
	public interface INamesData
	{
		public List<string> FirstNames { get; }
		public List<string> LastNames { get; }
	}
}
