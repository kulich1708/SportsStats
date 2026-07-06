using SportsStats.Application.Statistics;
using SportsStats.Domain.Matches;
using MediatR;
using SportsStats.Domain.Shared;
using SportsStats.Domain.Statistics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Matches
{
	public class MatchFinishHandler(
		TeamStatsService teamStatsService) : INotificationHandler<MatchFinishedNotification>
	{
		private readonly TeamStatsService _teamStatsService = teamStatsService;
		public async Task Handle(MatchFinishedNotification notification, CancellationToken ct)
		{
			await _teamStatsService.UpdateTeamsStatsAsync(notification.DomainEvent.MatchId);
		}

	}
}
