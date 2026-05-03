using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

namespace SportsStats.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddSportsStatsCore(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddDbContext<AppDbContext>(options =>
			options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

		// Repositories
		services.AddScoped<ITournamentRepository, TournamentRepository>();
		services.AddScoped<ITeamRepository, TeamRepository>();
		services.AddScoped<IMatchRepository, MatchRepository>();
		services.AddScoped<IPlayerRepository, PlayerRepository>();
		services.AddScoped<ITeamStatsRepository, TeamStatsRepository>();

		// Domain services
		services.AddScoped<ITimeProvider, SystemTimeProvider>();
		services.AddScoped<IMatchService, MatchService>();

		// Application services / handlers
		services.AddScoped<TournamentApplicationService>();
		services.AddScoped<PlayerApplicationService>();
		services.AddScoped<TeamApplicationService>();
		services.AddScoped<TeamStatsApplicationService>();
		services.AddScoped<MatchGoalService>();
		services.AddScoped<MatchFinishService>();
		services.AddScoped<MatchLifecycleService>();
		services.AddScoped<MatchRosterService>();
		services.AddScoped<MatchQueriesHandler>();

		return services;
	}
}