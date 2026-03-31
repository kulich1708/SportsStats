using Microsoft.OpenApi;
using System.Reflection;

namespace SportsStats.API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			ConfigureServices(builder);

			var app = builder.Build();

			ConfigureMiddleware(app);

			app.Run();
		}

		private static void ConfigureServices(WebApplicationBuilder builder)
		{
			var services = builder.Services;

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
					Description = "API для работы со спортивной статистикой"
				});

				// XML комментарии
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

					options.RoutePrefix = string.Empty;
				});
			}

			app.UseHttpsRedirection();
			app.UseAuthorization();
			app.MapControllers();
		}
	}
}