using SportsStats.Domain.Matches;
using SportsStats.Domain.Matches.Goals;
using SportsStats.Domain.Players;
using SportsStats.Domain.Tournaments;
using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using SportsStats.Domain.Services;
using SportsStats.Application.Matches.DTOs.Responses;

namespace SportsStats.Application.Matches
{
	public class MatchApplicationService(IPlayerRepository playerRepository,
		ITournamentRepository tournamentRepository, IMatchRepository matchRepository,
		ITimeProvider timeProvider)
	{
		private readonly IPlayerRepository _playerRepository = playerRepository;
		private readonly ITournamentRepository _tournamentRepository = tournamentRepository;
		private readonly IMatchRepository _matchRepository = matchRepository;
		private readonly ITimeProvider _timeProvider = timeProvider;

		public async Task<int> CreateAsync(int tournamentId, int homeTeamId, int awayTeamId)
		{
			Tournament tournament = await _tournamentRepository.GetAsync(tournamentId)
				?? throw new ArgumentException("Нет турнира с таким Id");

			Match match = new MatchCreationService().CreateMatch(tournament, homeTeamId, awayTeamId);
			await _matchRepository.AddAsync(match);
			await _matchRepository.SaveChangesAsync();
			return match.Id;
		}
		public async Task StartAsync(int matchId)
		{
			Match match = await GetMatchOrThrowAsync(matchId);

			match.Start(_timeProvider.GetCurrentTime());

			await _matchRepository.SaveChangesAsync();
		}
		public async Task FinishAsync(int matchId)
		{
			Match match = await GetMatchOrThrowAsync(matchId);

			match.Finish(_timeProvider.GetCurrentTime());

			await _matchRepository.SaveChangesAsync();
		}

		public async Task<GoalEvent> AddGoalAsync(int matchId, int scoringTeamId, int goalScorerId, int period, int time)
		{
			Match match = await GetMatchOrThrowAsync(matchId);

			GoalEvent goal = match.AddGoal(scoringTeamId, goalScorerId, period, time, _timeProvider.GetCurrentTime());

			await _matchRepository.SaveChangesAsync();
			return goal;
		}
		public async Task AddPlayerToRosterAsync(int matchId, int playerId, int teamId)
		{
			Match match = await GetMatchOrThrowAsync(matchId);
			Player player = await _playerRepository.GetAsync(playerId)
				?? throw new ArgumentException("Игрока с таким id не существует");

			if (player.TeamId != teamId)
				throw new ArgumentException("Игрок не состоит в данной команде");

			match.AddPlayerToRoster(playerId, teamId);

			await _matchRepository.SaveChangesAsync();
		}
		public async Task FillGoalDetailsAsync(int matchId, int goalId, int? firstAssistId, int? secondAssistId,
									GoalStrengthType strengthType, GoalNetType? netType = null)
		{
			Match match = await GetMatchOrThrowAsync(matchId);

			match.FillGoalDetails(goalId, firstAssistId, secondAssistId, strengthType, netType);

			await _matchRepository.SaveChangesAsync();
		}
		public async Task<MatchDTO?> GetAsync(int matchId)
		{
			Match? match = await _matchRepository.GetAsync(matchId);
			return match == null ? null : MatchMapper.ToDTO(match);
		}
		public async Task<List<MatchDTO>> GetAllAsync(int tournamentId, int? teamId = null)
		{
			return (await _matchRepository.GetAllAsync(tournamentId, teamId)).Select(MatchMapper.ToDTO).ToList();
		}


		private async Task<Match> GetMatchOrThrowAsync(int matchId)
		{
			return await _matchRepository.GetAsync(matchId)
				?? throw new ArgumentException($"Матч {matchId} не существует");
		}
	}
}
