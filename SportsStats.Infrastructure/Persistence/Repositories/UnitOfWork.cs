using Microsoft.EntityFrameworkCore;
using SportsStats.Domain.Common;
using SportsStats.Infrastructure.Persistence.DbContexts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Infrastructure.Persistence.Repositories
{
	public class UnitOfWork(AppDbContext context, IMediator mediator) : IUnitOfWork
	{
		private readonly AppDbContext _context = context;
		private readonly IMediator _mediator = mediator;

		public async Task SaveChangesAsync()
		{
			var aggregates = _context.ChangeTracker
				.Entries<AggregateRoot>()
				.Select(e => e.Entity)
				.Where(a => a.Events.Count > 0)
				.ToList();
			var events = aggregates.SelectMany(a => a.Events).ToList();

			var result = await _context.SaveChangesAsync();

			foreach (var a in aggregates)
				a.ClearEvents();

			foreach (var e in events)
				await _mediator.Publish(e);
		}
	}
}
