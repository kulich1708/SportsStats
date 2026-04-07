//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;

//namespace SportsStats.Infrastructure.Persistence.DbContexts
//{
//	public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
//	{
//		public AppDbContext CreateDbContext(string[] args)
//		{
//			var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

//			// Строка подключения для миграций
//			optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=SportsStats;Username=VladislavKulichkov;Password=qw06062013?");

//			return new AppDbContext(optionsBuilder.Options);
//		}
//	}
//}