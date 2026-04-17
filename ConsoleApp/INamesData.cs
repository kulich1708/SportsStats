using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{
	public interface INamesData
	{
		public List<string> FirstNames { get; }
		public List<string> LastNames { get; }
	}
}
