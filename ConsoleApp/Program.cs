using ConsoleApp.Players;
using ConsoleApp.Teams;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SportsStats.Application.Matches;
using SportsStats.Application.Players;
using SportsStats.Application.Players.DTOs.Responses;
using SportsStats.Application.Statistics;
using SportsStats.Application.Teams;
using SportsStats.Application.Teams.DTOs.Responses;
using SportsStats.Application.Tournaments;
using SportsStats.Domain.Matches;
using SportsStats.Domain.Players;
using SportsStats.Domain.Services;
using SportsStats.Domain.Shared;
using SportsStats.Domain.Statistics;
using SportsStats.Domain.Teams;
using SportsStats.Domain.Tournaments;
using SportsStats.Infrastructure.Persistence.DbContexts;
using SportsStats.Infrastructure.Persistence.Repositories;
using SportsStats.Infrastructure.Services;
using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using System.IO;
using System.Reflection;

namespace SportsStats.ConsoleApp
{
	public class DataGenerator
	{

		public async Task Start()
		{
			//Console.WriteLine("Генерация данных запущена");
			//if (!_context.Database.CanConnect())
			//	Console.WriteLine("Не удалось подключиться");

			//int nhlTournamentId = await GenerateTournamentAsync("NHL", new NHLTeamsNames(), new ForeignNamesData());
			//int khlTournamentId = await GenerateTournamentAsync("KHL", new KHLTeamNames(), new RussianNamesData());
		}
	}
	public static class Program
	{
		public static async Task Main()
		{
			//var test = new DataGenerator();
			//await test.Start();
			//var playerPhotoHelper = new PhotoHelper();
			//playerPhotoHelper.FillPhoto();

		}


	}
}
