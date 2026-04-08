using SportsStats.Domain.Matches;
using SportsStats.Domain.Matches.Goals;
using SportsStats.Domain.Players;
using SportsStats.Domain.Tournaments;
using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using SportsStats.Domain.Services;

namespace SportsStats.Application.Matches
{
	public class MatchApplicationService(IPlayerRepository playerRepository,
		ITournamentRepository tournamentRepository, IMatchRepository matchRepository,
		IGoalRepository goalRepository,
		ITimeProvider timeProvider)
	{
		private readonly IPlayerRepository _playerRepository = playerRepository;
		private readonly ITournamentRepository _tournamentRepository = tournamentRepository;
		private readonly IMatchRepository _matchRepository = matchRepository;
		private readonly IGoalRepository _goalRepository = goalRepository;
		private readonly ITimeProvider _timeProvider = timeProvider;

		public async Task<Match> CreateMatch(int tournamentId, int homeTeamId, int awayTeamId)
		{
			Tournament tournament = await _tournamentRepository.FindById(tournamentId)
				?? throw new ArgumentException("Нет турнира с таким Id");

			Match match = new MatchCreationService().CreateMatch(tournament, homeTeamId, awayTeamId);
			return await _matchRepository.Save(match);
		}
		public async Task StartMatch(int matchId)
		{
			Match match = await GetMatchOrThrow(matchId);

			match.Start(_timeProvider.GetCurrentTime());

			await _matchRepository.Save(match);
		}
		public async Task FinishMatch(int matchId)
		{
			Match match = await GetMatchOrThrow(matchId);

			match.Finish(_timeProvider.GetCurrentTime());

			await _matchRepository.Save(match);
		}

		public async Task<GoalEvent> AddGoal(int matchId, int scoringTeamId, int goalScorerId, int period, int time)
		{
			Match match = await GetMatchOrThrow(matchId);

			GoalEvent goal = match.AddGoal(scoringTeamId, goalScorerId, period, time, _timeProvider.GetCurrentTime());

			//foreach (var goal in match.UpdatedGoals)
			//	await _goalRepository.Save(goal);
			match.OnSaved();
			await _matchRepository.Save(match);
			return goal;
		}
		public async Task AddPlayerToRoster(int matchId, int playerId, int teamId)
		{
			Match match = await GetMatchOrThrow(matchId);
			Player player = await _playerRepository.FindById(playerId)
				?? throw new ArgumentException("Игрока с таким id не существует");

			if (player.TeamId != teamId)
				throw new ArgumentException("Игрок не состоит в данной команде");

			match.AddPlayerToRoster(playerId, teamId);

			await _matchRepository.Save(match);
		}
		public async Task FillGoalDetails(int matchId, int goalId, int? firstAssistId, int? secondAssistId,
									GoalStrengthType strengthType, GoalNetType? netType = null)
		{
			Match match = await GetMatchOrThrow(matchId);

			match.FillGoalDetails(goalId, firstAssistId, secondAssistId, strengthType, netType);

			//foreach (var goal in match.UpdatedGoals)
			//	await _goalRepository.Save(goal);
			match.OnSaved();
			await _matchRepository.Save(match);
		}
		private async Task<Match> GetMatchOrThrow(int matchId)
		{
			return await _matchRepository.FindById(matchId)
				?? throw new ArgumentException($"Матч {matchId} не существует");
		}
	}
}
