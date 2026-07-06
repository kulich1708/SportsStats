using MediatR;
using SportsStats.Application.Statistics.DTOs.Responses;
using SportsStats.Domain.Matches;
using SportsStats.Domain.Statistics;
using SportsStats.Domain.Teams;
using SportsStats.Domain.Tournaments;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Statistics
{
	public class TeamStatsService(
		ITeamStatsRepository teamStatsRepository,
		ITeamRepository teamRepository,
		ITournamentRepository tournamentRepository,
		IMatchRepository matchRepository)
	{
		private readonly ITeamStatsRepository _teamStatsRepository = teamStatsRepository;
		private readonly ITeamRepository _teamRepository = teamRepository;
		private readonly ITournamentRepository _tournamentRepository = tournamentRepository;
		private readonly IMatchRepository _matchRepository = matchRepository;
		public async Task<List<TeamStatsDTO>> GetByTeamAsync(int teamId)
		{
			List<TeamStats> stats = await _teamStatsRepository.GetByTeamAsync(teamId);

			var teamName = (await _teamRepository.GetAsync(teamId))!.Name;
			var tournamentIds = stats.Select(s => s.TournamentId).Distinct().ToList();
			var tournamentNames = (await _tournamentRepository.GetAsync(tournamentIds))
								  .ToDictionary(t => t.Id, t => t.Name);

			return stats.Select(s => TeamStatsMapper.ToDTO(
				s, teamName,
				tournamentNames.GetValueOrDefault(s.TournamentId)!
			)).ToList();
		}

		public async Task<List<TeamStatsDTO>> GetByTournamentAsync(int tournamentId)
		{
			List<TeamStats> stats = await _teamStatsRepository.GetByTournamentAsync(tournamentId);

			var tournamentName = (await _tournamentRepository.GetAsync(tournamentId))!.Name;
			var teamNames = (await _teamRepository.GetByTournamentAsync(tournamentId))
								  .ToDictionary(t => t.Id, t => t.Name);

			return stats.Select(s => TeamStatsMapper.ToDTO(
				s,
				teamNames.GetValueOrDefault(s.TeamId)!,
				tournamentName
			)).ToList();
		}
		public async Task UpdateTeamsStatsAsync(int matchId)
		{
			Match match = await _matchRepository.GetAsync(matchId)
				?? throw new ArgumentException("Невозможно выполнить пересчёт статистики, потому что не существует матча с таким id");
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
