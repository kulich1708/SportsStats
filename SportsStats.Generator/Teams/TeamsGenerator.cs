using ConsoleApp.Tools;
using SportsStats.Application.Teams;
using SportsStats.Generator.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ConsoleApp.Teams
{
	public class TeamsGenerator(
		TeamApplicationService teamApplicationService)
	{
		private readonly TeamApplicationService _teamApplicationService = teamApplicationService;
		public async Task<List<int>> GenerateTeamsAsync(ITeamsData teamsData, string directory, int? countLimit = null)
		{
			var sw = Stopwatch.StartNew();
			var data = teamsData.Data;
			int count = countLimit.HasValue ? Math.Min(data.Count, countLimit.Value) : data.Count;
			string basePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Teams", "Photos", directory));
			List<int> ids = new List<int>();

			for (int i = 0; i < count; i++)
			{
				var item = data[i];
				string photoPath = Path.Combine(basePath, $"{item.Item3}.png");
				int teamId = await GenerateTeamAsync(item.Item1, item.Item2, photoPath);
				ids.Add(teamId);
			}

			sw.Log("Созданы все команды");
			Console.WriteLine();
			return ids;
		}
		public async Task<int> GenerateTeamAsync(string name, string? city = null, string? photoPath = null)
		{
			var teamId = await _teamApplicationService.CreateAsync(name);

			byte[]? photo = null;
			string? photoMime = null;

			if (File.Exists(photoPath))
			{
				photo = File.ReadAllBytes(photoPath);
				photoMime = PhotoHelper.GetMimeTypeFromExtension(photoPath);
			}
			await _teamApplicationService.ChangeGeneralInfo(teamId, name, city, photo, photoMime);

			return teamId;
		}
	}
}
