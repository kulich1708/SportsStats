using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using SportsStats.Domain.Common;
using SportsStats.Domain.Matches;
using SportsStats.Domain.Players;
using SportsStats.Domain.Statistics;
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
		public DbSet<TeamStats> TeamsStats { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Tournament>(entity =>
			{
				entity.Property(t => t.TeamsId)
					  .HasField("_teamsId")
					  .HasColumnType("jsonb")
					  .HasConversion(
							v => JsonSerializer.Serialize(v.ToList()),
							v => new HashSet<int>(JsonSerializer.Deserialize<List<int>>(v)),
							new ValueComparer<IReadOnlySet<int>>(
								(c1, c2) => (c1 == c2) ||
					  					(c1 != null && c2 != null &&
					  					 c1.SetEquals(c2)),
								c => c.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
					  			c => new HashSet<int>(c)
							)
					  );

				entity.OwnsOne(t => t.TournamentRules, rulesBuilder =>
				{
					rulesBuilder.ToJson();

					rulesBuilder.OwnsOne(r => r.MatchDurationRules);
					rulesBuilder.OwnsOne(r => r.MatchRosterRules);
					rulesBuilder.OwnsOne(r => r.MatchPointsRules);
				});
			});

			modelBuilder.Entity<Match>(entity =>
			{
				entity.Property(match => match.HomeTeamRoster)
					.HasField("_homeTeamRoster")
					.HasColumnType("jsonb")
					.HasConversion(
							v => JsonSerializer.Serialize(v.ToList()),
							v => new HashSet<int>(JsonSerializer.Deserialize<List<int>>(v)),
							new ValueComparer<IReadOnlySet<int>>(
								(c1, c2) => (c1 == c2) ||
					  					(c1 != null && c2 != null &&
					  					 c1.SetEquals(c2)),
								c => c.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
					  			c => new HashSet<int>(c)
							)
					  );
				entity.Property(match => match.AwayTeamRoster)
					.HasField("_awayTeamRoster")
					.HasColumnType("jsonb")
					.HasConversion(
							v => JsonSerializer.Serialize(v.ToList()),
							v => new HashSet<int>(JsonSerializer.Deserialize<List<int>>(v)),
							new ValueComparer<IReadOnlySet<int>>(
								(c1, c2) => (c1 == c2) ||
					  					(c1 != null && c2 != null &&
					  					 c1.SetEquals(c2)),
								c => c.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
					  			c => new HashSet<int>(c)
							)
					  );

				entity.OwnsOne(m => m.Rules, rulesBuilder =>
				{
					rulesBuilder.ToJson();

					rulesBuilder.OwnsOne(r => r.MatchDurationRules);
					rulesBuilder.OwnsOne(r => r.MatchRosterRules);
					rulesBuilder.OwnsOne(r => r.MatchPointsRules);
				});
			});
		}
		//protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		//{
		//	optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
		//	optionsBuilder.EnableSensitiveDataLogging(); // Для отладки
		//}
	}
}
