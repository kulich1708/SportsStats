using Microsoft.EntityFrameworkCore;
using SportsStats.Domain.Players;
using SportsStats.Infrastructure.Persistence.DbContexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{
	public class PhotoHelper
	{
		private string _basePath = @"D:\программирование\Projects\Парсер и данные для статистики\Данные\KHL";
		private string _basePathForFlags = @"D:\программирование\Projects\Парсер и данные для статистики\Данные\Flags\RU.png";

		private readonly DbContextOptionsBuilder<AppDbContext> _options;
		private readonly AppDbContext _context;
		private readonly Random _random;

		public PhotoHelper()
		{
			_options = new DbContextOptionsBuilder<AppDbContext>()
					.UseNpgsql("Host=localhost;Database=SportsStats;Username=VladislavKulichkov;Password=qw06062013?");
			_context = new(_options.Options);

			_random = new Random();
		}
		public static string GetMimeTypeFromExtension(string filePath)
		{
			string extension = Path.GetExtension(filePath).ToLower();
			Console.WriteLine(extension);
			return extension switch
			{
				".png" => "image/png",
				".jpg" or ".jpeg" => "image/jpeg",
				".gif" => "image/gif",
				_ => "application/octet-stream", // неизвестный тип
			};
		}
		public void FillPhoto()
		{
			try
			{
				if (!Directory.Exists(_basePath))
				{
					Console.WriteLine($"Директория не найдена: {_basePath}");
					return;
				}

				var teamFolders = Directory.GetDirectories(_basePath);
				Console.WriteLine($"Найдено папок команд: {teamFolders.Length}");

				foreach (var teamFolder in teamFolders)
				{
					string teamIdString = Path.GetFileName(teamFolder);
					if (!int.TryParse(teamIdString, out int teamId))
					{
						Console.WriteLine($"Некорректный ID команды в имени папки: {teamIdString}");
						continue;
					}

					Console.WriteLine($"\nОбработка команды с ID: {teamId}");

					bool anyUpdate = false;
					var team = _context.Teams.FirstOrDefault(t => t.Id == teamId);
					if (team != null)
					{
						var logoPath = Path.Combine(teamFolder, "logo.png");
						if (File.Exists(logoPath))
							Console.WriteLine($"    Команда {teamId}: фото найдено - {Path.GetFileName(logoPath)}");

						byte[] logoBytes = File.ReadAllBytes(logoPath);
						team.SetPhoto(logoBytes, GetMimeTypeFromExtension(logoPath));
						anyUpdate = true;
					}
					else
					{
						Console.WriteLine($"    Команда (ID: {teamId}): фото не найдено по файл 'logo.png'");
					}

					var players = _context.Players
						.Where(p => p.TeamId == teamId)
						.OrderBy(p => p.Id)
						.ToList();

					if (players.Count == 0)
					{
						Console.WriteLine($"  Игроки для команды {teamId} не найдены");
						continue;
					}

					Console.WriteLine($"  Найдено игроков: {players.Count}");

					for (int i = 0; i < players.Count; i++)
					{
						var player = players[i];
						int playerNumber = i + 1;

						string photoPattern = $"{playerNumber}_*.jpg";
						string[] photoFiles = Directory.GetFiles(teamFolder, photoPattern);

						if (photoFiles.Length > 0)
						{
							string photoPath = photoFiles[0];
							Console.WriteLine($"    Игрок {playerNumber} (ID: {player.Id}): фото найдено - {Path.GetFileName(photoPath)}");

							byte[] photoBytes = File.ReadAllBytes(photoPath);
							player.SetPhoto(photoBytes, GetMimeTypeFromExtension(photoPath));
							anyUpdate = true;
						}
						else
						{
							Console.WriteLine($"    Игрок {playerNumber} (ID: {player.Id}): фото не найдено по маске '{photoPattern}'");
						}

						player.SetCitizenship("Россия", File.ReadAllBytes(_basePathForFlags), GetMimeTypeFromExtension(_basePathForFlags));

						int minDays = 18 * 365;
						int maxDays = 40 * 365;
						int daysAgo = _random.Next(minDays, maxDays + 1);
						DateOnly generatedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-daysAgo));
						Console.WriteLine(generatedDate.ToString("dd.MM.yyyy"));

						player.SetBirthday(generatedDate);
						player.SetNumber(playerNumber);
					}

					if (anyUpdate)
					{
						int savedCount = _context.SaveChanges();
						Console.WriteLine($"  Сохранено изменений для команды {teamId}: {savedCount}");

					}
				}

				Console.WriteLine("\nОбработка завершена!");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Ошибка: {ex.Message}");
				Console.WriteLine(ex.StackTrace);
			}
		}
	}
}
