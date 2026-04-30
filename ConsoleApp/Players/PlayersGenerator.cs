using ConsoleApp.Tools;
using SportsStats.Application.Players;
using SportsStats.Application.Players.DTOs.Requests;
using SportsStats.Application.Players.DTOs.Shared;
using SportsStats.Domain.Players;
using SportsStats.Domain.Teams;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp.Players
{
	public class PlayersGenerator(
		PlayerApplicationService playerApplicationService,
		INamesData namesData)
	{
		private readonly PlayerApplicationService _playerApplicationService = playerApplicationService;
		private readonly List<string> _names = namesData.FirstNames;
		private readonly List<string> _surnames = namesData.LastNames;
		private readonly Random _random = new();

		private string _basePath = @".\Photos\KHL";
		private string _basePathForFlags = @".\Photos\Flags";

		private static int _count = 1;
		public async Task<Dictionary<int, List<int>>> GeneratePlayersForTeamsAsync(List<int> teamIds, bool isRussian = true)
		{
			Dictionary<int, List<int>> players = new();

			foreach (int teamId in teamIds)
				players.Add(teamId, await GeneratePlayersForTeamAsync(teamId));

			return players;
		}
		public async Task<List<int>> GeneratePlayersForTeamAsync(int teamId, bool isRussian = true)
		{
			List<int> players = new();
			Dictionary<PositionType, int> positionsCount = new()
			{
				{PositionType.LeftWinger, 4},
				{PositionType.RightWinger, 4},
				{PositionType.Center, 4},
				{PositionType.LeftDefenseman, 3},
				{PositionType.LeftDefenseman, 3},
				{PositionType.Goalie, 2},
			};

			int count = 0;
			int[] numbers = Enumerable.Range(1, 99).OrderBy(_random.Next).ToArray();
			foreach (var item in positionsCount)
				for (int i = 0; i < item.Value; i++)
					players.Add(await GeneratePlayerAsync(item.Key, teamId, numbers[count++], isRussian));

			return players;
		}
		public async Task<int> GeneratePlayerAsync(PositionType position, int teamId, int number, bool isRussian = true)
		{
			int nameIndex = _random.Next(_names.Count);
			int surnameIndex = _random.Next(_surnames.Count);
			string name = _names[nameIndex];
			string surname = _surnames[surnameIndex];

			int minDays = 18 * 365;
			int maxDays = 40 * 365;
			int daysAgo = _random.Next(minDays, maxDays + 1);
			DateOnly birthday = DateOnly.FromDateTime(DateTime.Today.AddDays(-daysAgo));

			byte[]? photo = null;
			string? photoMime = null;

			string filePath = Path.Combine(_basePath, @$"{_count++ % Directory.GetFiles(_basePath).Length}.jpg");
			if (Directory.Exists(_basePath) &&
				File.Exists(filePath))
			{
				photo = File.ReadAllBytes(filePath);
				photoMime = PhotoHelper.GetMimeTypeFromExtension(filePath);
			}

			int playerId = await _playerApplicationService.CreateAsync(name, surname, position);
			PlayerGeneralInfoDTO info = new(
				name,
				surname,
				position,
				teamId,
				number,
				birthday,
				GenerateCitizenship(),
				photo,
				photoMime
			);
			await _playerApplicationService.ChangeGeneralInfoAsync(playerId, info);

			return playerId;
		}
		private CitizenshipDTO GenerateCitizenship()
		{
			string name = "Россия";
			byte[]? photo = null;
			string? photoMime = null;

			string filePath = Path.Combine(_basePathForFlags, @"RU.png");
			if (Directory.Exists(_basePathForFlags) &&
				File.Exists(filePath))
			{
				photo = File.ReadAllBytes(filePath);
				photoMime = PhotoHelper.GetMimeTypeFromExtension(filePath);
			}

			return new(name, photo, photoMime);
		}
	}
}
