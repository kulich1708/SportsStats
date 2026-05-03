using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SportsStats.Generator.Tools
{
	public static class StopwatchExtensions
	{
		public static void Log(this Stopwatch sw, string message)
		{
			sw.Stop();
			Console.WriteLine($"{message}: {sw.ElapsedMilliseconds} мс");
			sw.Restart();
		}
	}
}
