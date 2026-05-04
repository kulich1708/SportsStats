using ConsoleApp;
using ConsoleApp.Matches;
using ConsoleApp.Players;
using ConsoleApp.Teams;
using ConsoleApp.Tools;
using ConsoleApp.Tournaments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SportsStats.Infrastructure;
using SportsStats.Infrastructure.Persistence.DbContexts;
using System.Diagnostics;
using System.Text;

namespace SportsStats.ConsoleApp
{
	public static class Program
	{
		public static async Task Main()
		{
			string baseDir = AppDomain.CurrentDomain.BaseDirectory;
			var appSettingsPath = Path.Combine(baseDir, "appsettings.json");

			var configuration = new ConfigurationBuilder()
				.AddJsonFile(appSettingsPath, optional: false, reloadOnChange: false)
				.AddEnvironmentVariables()
				.Build();

			var services = new ServiceCollection();

			services.AddSportsStatsCore(configuration);
			services.AddScoped<AllGenerator>();
			services.AddScoped<PlayersGenerator>();
			services.AddScoped<TeamsGenerator>();
			services.AddScoped<TournamentGenerator>();
			services.AddScoped<MatchGenerator>();
			services.AddScoped<GoalsGenerator>();

			await using var provider = services.BuildServiceProvider();
			await using var scope = provider.CreateAsyncScope();

			var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
			await db.Database.MigrateAsync();

			var generator = scope.ServiceProvider.GetRequiredService<AllGenerator>();
			await generator.Start();
		}
	}
}
