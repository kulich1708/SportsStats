using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using SportsStats.Domain.Common;
using SportsStats.Domain.Matches;
using SportsStats.Domain.Players;
using SportsStats.Domain.Statistics;
using SportsStats.Domain.Teams;
using SportsStats.Domain.Tournaments;
using SportsStats.Domain.Tournaments.Rules;
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
				entity.Property(e => e.Photo)
					  .HasColumnType("bytea");

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
				entity.Property(t => t.TournamentRules)
					  .HasColumnType("jsonb")
					  .HasConversion(
						  v => v == null ? null : JsonSerializer.Serialize(v),
						  v => v == null ? null : JsonSerializer.Deserialize<TournamentRules>(v)
					  );
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


				entity.Property(t => t.Rules)
					  .HasColumnType("jsonb")
					  .HasConversion(
						  v => v == null ? null : JsonSerializer.Serialize(v),
						  v => v == null ? null : JsonSerializer.Deserialize<TournamentRules>(v)
					  );
			});

			modelBuilder.Entity<Team>(entity =>
			{
				entity.Property(e => e.Photo)
					  .HasColumnType("bytea");
			});

			modelBuilder.Entity<Player>(entity =>
			{
				entity.Property(e => e.Photo)
					  .HasColumnType("bytea");

				entity.ComplexProperty(p => p.Citizenship, citizenship =>
				{
					citizenship.Property(c => c.Name)
							   .HasColumnName("CitizenshipName");
					citizenship.Property(c => c.Photo)
							   .HasColumnName("CitizenshipPhoto")
							   .HasColumnType("bytea");
				});
				entity.ComplexProperty(p => p.Number, number =>
				{
					number.Property(n => n.Number)
						  .HasColumnName("Number");
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
