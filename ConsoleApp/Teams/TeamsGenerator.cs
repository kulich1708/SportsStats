using ConsoleApp.Players;
using SportsStats.Application.Teams;
using SportsStats.Domain.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp.Teams
{
	public class TeamsGenerator(TeamApplicationService teamApplicationService, string directory)
	{
		private readonly TeamApplicationService _teamApplicationService = teamApplicationService;

		private string _basePath = Path.Combine("./Photos", directory);
		public async Task<List<int>> GenerateTeamsAsync(ITeamsData teamData)
		{
			var data = teamData.Data;

			List<int> ids = new List<int>();
			foreach (var item in data)
			{
				var teamId = await _teamApplicationService.CreateAsync(item.Item1);

				byte[]? photo = null;
				string? photoMime = null;

				string filePath = Path.Combine(_basePath, @$"{item.Item3}.png");
				if (Directory.Exists(_basePath) &&
					File.Exists(filePath))
				{
					photo = File.ReadAllBytes(filePath);
					photoMime = PhotoHelper.GetMimeTypeFromExtension(filePath);
				}
				await _teamApplicationService.ChangeGeneralInfo(teamId, item.Item1, item.Item2, photo, photoMime);
				ids.Add(teamId);
			}
			return ids;
		}
	}
}
