using SportsStats.Domain.Matches;
using SportsStats.Domain.Shared;
using SportsStats.Domain.Statistics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Matches
{
	public class MatchFinishService(
		ITeamStatsRepository teamStatsRepository,
		IMatchRepository matchRepository,
		ITimeProvider timeProvider) : MatchUseCaseBase(matchRepository)
	{
		private readonly ITeamStatsRepository _teamStatsRepository = teamStatsRepository;
		private readonly IMatchRepository _matchRepository = matchRepository;
		private readonly ITimeProvider _timeProvider = timeProvider;


		public async Task FinishAsync(int matchId, DateTime? finishedAt = null)
		{
			Match match = await GetMatchOrThrowAsync(matchId);

			match.Finish(finishedAt ?? _timeProvider.GetCurrentTime());

			await _matchRepository.SaveChangesAsync();

			await UpdateTeamsStatsAsync(match);
		}
		private async Task UpdateTeamsStatsAsync(Match match)
		{

			TeamStats homeTeamStats = await _teamStatsRepository.GetAsync(match.HomeTeamId, match.TournamentId);
			TeamStats awayTeamStats = await _teamStatsRepository.GetAsync(match.AwayTeamId, match.TournamentId);
			int? homeTeamPoint = match.Rules.MatchPointsRules.GetPoints(match.HomeTeamWinType);
			int? awayTeamPoint = match.Rules.MatchPointsRules.GetPoints(match.AwayTeamWinType);
			if (!homeTeamPoint.HasValue || !awayTeamPoint.HasValue)
				throw new ArgumentException("Найдено несовпадение. Для команд(ы) установлен исход, для которого не установлнено количество очков. Проверьте события и правила матча");

			homeTeamStats.AddOutcome(match.HomeTeamWinType, homeTeamPoint.Value);
			awayTeamStats.AddOutcome(match.AwayTeamWinType, awayTeamPoint.Value);

			await _teamStatsRepository.SaveChangesAsync();
		}
	}
}
