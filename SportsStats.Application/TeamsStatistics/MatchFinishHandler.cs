using SportsStats.Application.Statistics;
using SportsStats.Domain.Matches;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.TeamsStatistics
{
	public class MatchFinishHandler(
		TeamStatsService teamStatsService) : INotificationHandler<MatchFinishedEvent>
	{
		private readonly TeamStatsService _teamStatsService = teamStatsService;
		public async Task Handle(MatchFinishedEvent @event, CancellationToken ct)
		{
			await _teamStatsService.UpdateTeamsStatsAsync(@event.MatchId);
		}

	}
}
