using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
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
					  .HasField("_teamsId")
					  .HasColumnType("jsonb")
					  .HasConversion(
							v => JsonSerializer.Serialize(v),
							v => JsonSerializer.Deserialize<List<int>>(v),
							new ValueComparer<IReadOnlyList<int>>(
								(c1, c2) => (c1 == c2) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
								c => c.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
								c => c.ToList()  // Для снимка создаём List, но он будет обёрнут в IReadOnlyList
							)
					  );

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

			modelBuilder.Entity<Match>(entity =>
			{
				entity.Property(match => match.HomeTeamRoster)
					.HasColumnType("jsonb")
					.HasConversion(
						p => JsonSerializer.Serialize(p),
						p => JsonSerializer.Deserialize<List<int>>(p));
				entity.Property(match => match.AwayTeamRoster)
					.HasColumnType("jsonb")
					.HasConversion(
						p => JsonSerializer.Serialize(p),
						p => JsonSerializer.Deserialize<List<int>>(p));
			});
		}
		//protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		//{
		//	optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
		//	optionsBuilder.EnableSensitiveDataLogging(); // Для отладки
		//}
	}
}
