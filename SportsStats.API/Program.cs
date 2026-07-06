using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using SportsStats.API.Middleware;
using SportsStats.Application.Matches;
using SportsStats.Infrastructure;
using SportsStats.Infrastructure.Persistence.DbContexts;
using System.Reflection;

namespace SportsStats.API
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			ConfigureServices(builder);

			var app = builder.Build();
			await ConfigureMiddleware(app);

			app.Run();
		}

		private static void ConfigureServices(WebApplicationBuilder builder)
		{
			var services = builder.Services;

			//services.AddControllers()
			//	.AddJsonOptions(options =>
			//	{
			//		options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
			//	});


			services.AddDbContext<AppDbContext>(options =>
				options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
			);
			services.AddSportsStatsCore(builder.Configuration);

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

					Description = "API бэкенда хоккейной статисктики"
				});

				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

				if (File.Exists(xmlPath))
				{
					options.IncludeXmlComments(xmlPath);
				}
			});
			services.AddMediatR(cfg =>
			{
				cfg.RegisterServicesFromAssembly(typeof(MatchFinishHandler).Assembly);
			});
		}

		private static async Task ConfigureMiddleware(WebApplication app)
		{
			using var scope = app.Services.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
			await db.Database.MigrateAsync();

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
			app.UseMiddleware<GlobalExceptionHandler>();


			app.UseDefaultFiles();
			app.UseStaticFiles();

			app.UseHttpsRedirection();
			app.UseAuthorization();
			app.MapControllers();
			app.MapFallbackToFile("index.html");
		}
	}
}
