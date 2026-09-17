using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Infrastructure.Persistence.Repositories
{
	public abstract class BaseRepository<TAggregate> where TAggregate : class
	{
		public abstract Task<TAggregate?> FindByIdAsync(int id);
		protected abstract ErrorCode NotFoundErrorCode { get; }
		public async Task<TAggregate> GetByIdAsync(int id)
		{
			var entity = await FindByIdAsync(id);
			return entity ?? throw new NotFoundException(NotFoundErrorCode, id);
		}
	}
}
