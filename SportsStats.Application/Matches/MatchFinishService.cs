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
		IMatchRepository matchRepository) : MatchUseCaseBase(matchRepository)
	{
		private readonly ITeamStatsRepository _teamStatsRepository = teamStatsRepository;
		private readonly IMatchRepository _matchRepository = matchRepository;


		public async Task FinishAsync(int matchId, DateTime? finishedAt = null)
		{
			Match match = await GetMatchOrThrowAsync(matchId);

			match.Finish(finishedAt ?? match.StartedAt.Value.AddHours(2).AddMinutes(30));

			await _matchRepository.SaveChangesAsync();

			await UpdateTeamsStatsAsync(match);
		}
		private async Task UpdateTeamsStatsAsync(Match match)
		{

			TeamStats homeTeamStats = await _teamStatsRepository.GetAsync(match.HomeTeam.Id, match.TournamentId);
			TeamStats awayTeamStats = await _teamStatsRepository.GetAsync(match.AwayTeam.Id, match.TournamentId);
			int? homeTeamPoint = match.Rules.MatchPointsRules.GetPoints(match.HomeTeam.WinType);
			int? awayTeamPoint = match.Rules.MatchPointsRules.GetPoints(match.AwayTeam.WinType);
			if (!homeTeamPoint.HasValue || !awayTeamPoint.HasValue)
				throw new ArgumentException("Найдено несовпадение. Для команд(ы) установлен исход, для которого не установлнено количество очков. Проверьте события и правила матча");

			homeTeamStats.AddOutcome(match.HomeTeam.WinType, homeTeamPoint.Value);
			awayTeamStats.AddOutcome(match.AwayTeam.WinType, awayTeamPoint.Value);

			await _teamStatsRepository.SaveChangesAsync();
		}
	}
}
