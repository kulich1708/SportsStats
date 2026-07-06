using MediatR;
using SportsStats.Domain.Matches;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Matches
{
	public record MatchFinishedNotification(MatchFinishedEvent DomainEvent) : INotification
	{
	}
}
