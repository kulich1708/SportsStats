using SportsStats.Domain.Shared;
using SportsStats.Infrastructure.Persistence.DbContexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Infrastructure.Persistence.Repositories
{
	public abstract class BaseRepository<TAggregate>(AppDbContext context) where TAggregate : class
	{
		private readonly AppDbContext _context = context;
		public abstract Task<TAggregate?> FindByIdAsync(int id);
		protected abstract ErrorCode NotFoundErrorCode { get; }
		public async Task<TAggregate> GetByIdAsync(int id)
		{
			var entity = await FindByIdAsync(id);
			return entity ?? throw new NotFoundException(NotFoundErrorCode, id);
		}
		public void Add(TAggregate aggregate) => _context.Set<TAggregate>().Add(aggregate);
	}
}
