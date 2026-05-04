using ConsoleApp.Players;
using ConsoleApp.Teams;
using ConsoleApp.Tournaments;
using Microsoft.EntityFrameworkCore;
using SportsStats.Generator.Tools;
using SportsStats.Infrastructure.Persistence.DbContexts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ConsoleApp
{
	public class AllGenerator(
		PlayersGenerator playersGenerator,
		TeamsGenerator teamsGenerator,
		TournamentGenerator tournamentGenerator,
		AppDbContext appDbContext
		)
	{
		private readonly PlayersGenerator _playersGenerator = playersGenerator;
		private readonly TeamsGenerator _teamsGenerator = teamsGenerator;
		private readonly TournamentGenerator _tournamentGenerator = tournamentGenerator;
		private readonly AppDbContext _appDbContext = appDbContext;
		public async Task<int> GenerateTournamentAsync(
			string tournamentName, string directory, ITeamsData teamsData,
			INamesData playersData, bool isRussian = true, int? countTeamsLimit = null)
		{
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine($"Генерация данных для турнира {tournamentName} начата");
			var sw = Stopwatch.StartNew();

			var teams = await _teamsGenerator.GenerateTeamsAsync(teamsData, directory, countTeamsLimit);
			var players = await _playersGenerator.GeneratePlayersForTeamsAsync(teams, playersData, isRussian);
			var tournament = await _tournamentGenerator.GenerateTournamentAsync(tournamentName, teams);

			sw.Log($"Генерация данных для турнира {tournamentName} завершена");
			return tournament;
		}
		public async Task Start()
		{
			var sw = Stopwatch.StartNew();
			Console.WriteLine("Очистка базы данных");
			await ClearDatabaseAsync();
			sw.Log("Очистка базы данных завершена");
			Console.WriteLine();

			Console.WriteLine("Генерация данных начата");

			int khl = await GenerateTournamentAsync("KHL", "KHL", new KHLTeamNames(), new RussianNamesData(), true, 10);
			int nhl = await GenerateTournamentAsync("NHL", "NHL", new NHLTeamsNames(), new ForeignNamesData(), false, 5);

			sw.Log("Генерация данных завершена. Общее время генерации");
		}
		public async Task ClearDatabaseAsync()
		{
			await _appDbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"GoalEvent\"");
			await _appDbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"Matches\"");
			await _appDbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"Players\"");
			await _appDbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"Teams\"");
			await _appDbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"Tournaments\"");
			await _appDbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"TeamsStats\"");
		}
	}
}
