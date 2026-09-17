using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Infrastructure.Persistence.Repositories
{
	public abstract class BaseRepository<TAggregate> where TAggregate : class
	{
		protected abstract Task<TAggregate> FindByIdAsync(int id);
		protected abstract ErrorCode NotFoundError { get; }
		public async Task<TAggregate> GetByIdAsync(int id)
		{
			var entity = await FindByIdAsync(id);
			return entity ?? throw new NotFoundException(NotFoundError, id);
		}
	}
}
