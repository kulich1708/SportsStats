using SportsStats.Domain.Matches;
using SportsStats.Domain.Matches.Goals;
using SportsStats.Domain.Players;
using SportsStats.Domain.Tournaments;
using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Matches
{
	public class MatchApplicationService
	{
		private IPlayerRepository _playerRepository;
		private ITournamentRepository _tournamentRepository;
		private IMatchRepository _matchRepository;
		private IGoalRepository _goalRepository;
		private ITimeProvider _timeProvider;

		public MatchApplicationService(IPlayerRepository playerRepository,
			ITournamentRepository tournamentRepository, IMatchRepository matchRepository,
			IGoalRepository goalRepository,
			ITimeProvider timeProvider)
		{
			_playerRepository = playerRepository;
			_tournamentRepository = tournamentRepository;
			_matchRepository = matchRepository;
			_goalRepository = goalRepository;
			_timeProvider = timeProvider;
		}

		public Match CreateMatch(int tournamentId, int homeTeamId, int awayTeamId)
		{
			Tournament tournament = _tournamentRepository.FindById(tournamentId);
			List<int> tournamentTeams = tournament.TeamsId.ToList();

			if (tournament == null)
				throw new ArgumentException("Нет турнира с таким Id");

			if (tournament.Status == TournamentStatus.Finished)
				throw new ArgumentException("Нельзя добавить матч в законченный турнир");
			if (tournament.Status == TournamentStatus.Draft)
				throw new ArgumentException("Турнир ещё закрыт для добавления матчей");

			if (!tournamentTeams.Contains(homeTeamId) || !tournamentTeams.Contains(awayTeamId))
				throw new ArgumentException("Команда не заявлена на этот чемпионат");

			Match match = new Match(tournamentId, homeTeamId, awayTeamId, tournament.TournamentRules);
			return _matchRepository.Save(match);
		}
		public void StartMatch(int matchId)
		{
			Match match = GetMatchOrThrow(matchId);

			match.Start(_timeProvider.GetCurrentTime());

			_matchRepository.Save(match);
		}
		public void FinishMatch(int matchId)
		{
			Match match = GetMatchOrThrow(matchId);

			match.Finish(_timeProvider.GetCurrentTime());

			_matchRepository.Save(match);
		}

		public void AddGoal(int matchId, int scoringTeamId, int goalScorerId, int period, int time)
		{
			Match match = GetMatchOrThrow(matchId);

			match.AddGoal(scoringTeamId, goalScorerId, period, time, _timeProvider.GetCurrentTime());

			foreach (var goal in match.UpdatedGoals)
				_goalRepository.Save(goal);
			match.OnSaved();
			_matchRepository.Save(match);

		}
		public void AddPlayerToRoster(int matchId, int playerId, int teamId)
		{
			Match match = GetMatchOrThrow(matchId);
			Player player = _playerRepository.FindById(playerId);

			if (player == null)
				throw new ArgumentException("Игрока с таким id не существует");

			if (player.TeamId != teamId)
				throw new ArgumentException("Игрок не состоит в данной команде");

			match.AddPlayerToRoster(playerId, teamId);

			_matchRepository.Save(match);
		}
		public void FillGoalDetails(int matchId, int goalId, int? firstAssistId, int? secondAssistId,
									GoalStrengthType strengthType, GoalNetType? netType = null)
		{
			Match match = GetMatchOrThrow(matchId);

			match.FillGoalDetails(goalId, firstAssistId, secondAssistId, strengthType, netType);

			foreach (var goal in match.UpdatedGoals)
				_goalRepository.Save(goal);
			match.OnSaved();
			_matchRepository.Save(match);
		}
		private Match GetMatchOrThrow(int matchId)
		{
			return _matchRepository.FindById(matchId)
				?? throw new ArgumentException($"Матч {matchId} не существует");
		}
	}
}
