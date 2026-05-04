using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp.Tools
{
	public static class PathManager
	{
		public static string GetSolutionPath()
		{
			return Path.GetFullPath(@"..\");
		}
		public static string GetGeneratorPath()
		{
			return Path.Combine(GetSolutionPath(), "SportsStats.Generator");
		}
		public static string GetApiPath()
		{
			return Path.Combine(GetSolutionPath(), "SportsStats.API");
		}
	}
}
