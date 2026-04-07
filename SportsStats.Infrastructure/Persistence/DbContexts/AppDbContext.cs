using Microsoft.EntityFrameworkCore;
using SportsStats.Domain.Common;
using SportsStats.Domain.Matches;
using SportsStats.Domain.Players;
using SportsStats.Domain.Teams;
using SportsStats.Domain.Tournaments;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SportsStats.Infrastructure.Persistence.DbContexts
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options)
		{
		}
		public DbSet<Tournament> Tournaments { get; set; }

		public DbSet<Team> Teams { get; set; }
		public DbSet<Player> Players { get; set; }
		public DbSet<Match> Matches { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Tournament>(entity =>
			{
				entity.Property(t => t.TeamsId)
					.HasColumnType("jsonb")
					.HasConversion(
						v => JsonSerializer.Serialize(v),
						v => JsonSerializer.Deserialize<List<int>>(v));

				// Конфигурация корневого VO
				entity.OwnsOne(t => t.TournamentRules, rulesBuilder =>
				{
					// Включаем JSON для PostgreSQL
					rulesBuilder.ToJson();

					// Настраиваем вложенные VO
					rulesBuilder.OwnsOne(r => r.MatchDurationRules);
					rulesBuilder.OwnsOne(r => r.MatchRosterRules);
					rulesBuilder.OwnsOne(r => r.MatchPointsRules);
				});
			});
		}
	}
}
