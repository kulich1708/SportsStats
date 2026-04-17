using SportsStats.Domain.Matches;
using SportsStats.Domain.Players;
using SportsStats.Domain.Teams;
using SportsStats.Domain.Tournaments;
using SportsStats.Domain.Tournaments.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Services
{
	public class MatchService : IMatchService
	{

		public Match CreateMatch(Tournament tournament, int homeTeamId, int awayTeamId, DateTime scheduledAt)
		{
			if (!tournament.IsRegistration() && !tournament.IsStarted())
				throw new ArgumentException("Нельзя создать матч в турнире, который ещё не открыт");
			if (!IsTeamInTournament(tournament, homeTeamId))
				throw new ArgumentException("Домашняя команда не заявлена на турнир");
			if (!IsTeamInTournament(tournament, awayTeamId))
				throw new ArgumentException("Гостевая команда не заявлена на турнир");

			Match match = new Match(tournament.Id, homeTeamId, awayTeamId, tournament.TournamentRules, scheduledAt);

			return match;

		}
		public void Start(Match match, Tournament tournament, List<Player> homeTeamRoster, List<Player> awayTeamRoster, Team homeTeam, Team awayTeam, DateTime startedAt)
		{
			ValidateRoster(match, homeTeamRoster, homeTeam.Name);
			ValidateRoster(match, awayTeamRoster, awayTeam.Name);
			if (!tournament.IsStarted())
				throw new ArgumentException("Нельзя начать матч в неначатом турнире");
			match.Start(startedAt);
		}
		private void ValidateRoster(Match match, List<Player> roster, string teamName)
		{
			MatchRosterRules rules = match.Rules.MatchRosterRules;
			List<PositionType> forwardPositions = new() { PositionType.RightWinger, PositionType.LeftWinger, PositionType.Center };
			List<PositionType> defensemanPositions = new() { PositionType.RightDefenseman, PositionType.LeftDefenseman };
			List<PositionType> goaliePositions = new() { PositionType.Goalie };

			int forwardsCount = roster.Where(p => forwardPositions.Contains(p.Position)).Count();
			int defensemanCount = roster.Where(p => defensemanPositions.Contains(p.Position)).Count();
			int goalieCount = roster.Where(p => goaliePositions.Contains(p.Position)).Count();
			int playersCount = roster.Count;

			if (forwardsCount < rules.MinForwards || forwardsCount > rules.MaxForwards)
				throw new ArgumentException($"Проверьте состав команды {teamName}. По правилам турнира нападающих должно быть от {rules.MinForwards} до {rules.MaxForwards}");
			if (defensemanCount < rules.MinDefensemans || defensemanCount > rules.MaxDefensemans)
				throw new ArgumentException($"Проверьте состав команды {teamName}. По правилам турнира защитников должно быть от {rules.MinDefensemans} до {rules.MaxDefensemans}");
			if (goalieCount < rules.MinGoalies || goalieCount > rules.MaxGoalies)
				throw new ArgumentException($"Проверьте состав команды {teamName}. По правилам турнира вратарей должно быть от {rules.MinGoalies} до {rules.MaxGoalies}");
			if (playersCount < rules.MinPlayers || playersCount > rules.MaxPlayers)
				throw new ArgumentException($"Проверьте состав команды {teamName}. По правилам турнира игроков должно быть от {rules.MinPlayers} до {rules.MaxPlayers}");

		}
		private bool IsTeamInTournament(Tournament tournament, int teamId) => tournament.TeamsId.Contains(teamId);
	}
}
