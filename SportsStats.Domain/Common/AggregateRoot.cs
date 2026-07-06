using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Common
{
	public abstract class AggregateRoot : BaseEntity, IAggregateRoot
	{
		private readonly List<IDomainEvent> _events = new();

		public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();

		public void ClearEvents() => _events.Clear();

		protected void AddEvent(IDomainEvent @event) => _events.Add(@event);
	}
}

