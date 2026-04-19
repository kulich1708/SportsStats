using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using SportsStats.API.Middleware;
using SportsStats.Application.Matches;
using SportsStats.Application.Players;
using SportsStats.Application.Statistics;
using SportsStats.Application.Teams;
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
using System.Reflection;
using System.Text.Json.Serialization;

namespace SportsStats.API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);


			// ГЇВїВЅГЇВїВЅГЇВїВЅГЇВїВЅГЇВїВЅГЇВїВЅ ГЇВїВЅГЇВїВЅГЇВїВЅ ГЇВїВЅГЇВїВЅГЇВїВЅГЇВїВЅГЇВїВЅГЇВїВЅГЇВїВЅ ГЇВїВЅГЇВїВЅГЇВїВЅ ГЇВїВЅГЇВїВЅ
			builder.Services.AddDbContext<AppDbContext>(options =>
				options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
			);
			ConfigureServices(builder);

			var app = builder.Build();

			ConfigureMiddleware(app);

			app.Run();
		}

		private static void ConfigureServices(WebApplicationBuilder builder)
		{
			var services = builder.Services;

			services.AddControllers()
				.AddJsonOptions(options =>
				{
					options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
				});

			services.AddScoped<ITournamentRepository, TournamentRepository>();
			services.AddScoped<ITeamRepository, TeamRepository>();
			services.AddScoped<IMatchRepository, MatchRepository>();
			services.AddScoped<IPlayerRepository, PlayerRepository>();
			services.AddScoped<ITeamStatsRepository, TeamStatsRepository>();
			services.AddScoped<ITimeProvider, SystemTimeProvider>();
			services.AddScoped<IMatchService, MatchService>();

			services.AddScoped<TournamentApplicationService>();

			services.AddScoped<PlayerApplicationService>();

			services.AddScoped<TeamApplicationService>();
			services.AddScoped<TeamStatsApplicationService>();

			services.AddScoped<MatchGoalService>();
			services.AddScoped<MatchFinishService>();
			services.AddScoped<MatchLifecycleService>();
			services.AddScoped<MatchRosterService>();
			services.AddScoped<MatchQueriesHandler>();

			services.AddControllers();
			services.AddEndpointsApiExplorer();

			var version = Assembly.GetExecutingAssembly()
								  .GetName()
								  .Version?
								  .ToString() ?? "v1";

			services.AddSwaggerGen(options =>
			{
				options.SwaggerDoc(version, new OpenApiInfo
				{
					Title = "Sports Stats API",
					Version = version,

					Description = "API ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½"
				});

				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

				if (File.Exists(xmlPath))
				{
					options.IncludeXmlComments(xmlPath);
				}
			});
		}

		private static void ConfigureMiddleware(WebApplication app)
		{
			app.UseMiddleware<GlobalExceptionHandler>();
			if (app.Environment.IsDevelopment())
			{
				var version = Assembly.GetExecutingAssembly()
									  .GetName()
									  .Version?
									  .ToString() ?? "v1";

				app.UseSwagger();

				app.UseSwaggerUI(options =>
				{
					options.SwaggerEndpoint(
						$"/swagger/{version}/swagger.json",
						$"Sports Stats API {version}");

					options.RoutePrefix = "swagger";
				});
			}

			app.UseDefaultFiles();
			app.UseStaticFiles();

			app.UseHttpsRedirection();
			app.UseAuthorization();
			app.MapControllers();
			app.MapFallbackToFile("index.html");
		}
	}
}