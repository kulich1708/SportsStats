using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SportsStats.Domain.Players;

namespace SportsStats.Infrastructure.Persistence.DbContexts
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options)
		{
		}

		// Это свойство = таблица Products в базе данных
		public DbSet<Player> Products { get; set; }
	}
}
