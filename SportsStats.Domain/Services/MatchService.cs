using SportsStats.Domain.Matches;
using SportsStats.Domain.Players;
using SportsStats.Domain.Shared;
using SportsStats.Domain.Teams;
using SportsStats.Domain.Tournaments;
using SportsStats.Domain.Tournaments.Rules;
using SportsStats.Domain.Tournaments.Rules.MatchRoster;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Services
{
	public class MatchService : IMatchService
	{

		public Match CreateMatch(Tournament tournament, int homeTeamId, int awayTeamId, DateTime scheduledAt, TournamentRules rules)
		{
			if (!tournament.IsRegistration() && !tournament.IsStarted())
				throw new DomainException(MatchServiceError.MatchCanOnlyBeCreatedInRegistrationOrStarted);
			if (tournament.StartedAt > scheduledAt)
				throw new DomainException(MatchServiceError.MatchScheduleTimeCannotBeBeforeTournamentStart, scheduledAt, tournament.StartedAt.Value);
			if (!IsTeamInTournament(tournament, homeTeamId))
				throw new DomainException(MatchServiceError.HomeTeamNotRegistered);
			if (!IsTeamInTournament(tournament, awayTeamId))
				throw new DomainException(MatchServiceError.AwayTeamNotRegistered);


			Match match = new Match(tournament.Id, homeTeamId, awayTeamId, rules, scheduledAt);

			return match;

		}
		public void Start(Match match, Tournament tournament, List<Player> homeTeamRoster, List<Player> awayTeamRoster, Team homeTeam, Team awayTeam, DateTime startedAt)
		{
			ValidateRoster(match, homeTeamRoster, homeTeam.Name);
			ValidateRoster(match, awayTeamRoster, awayTeam.Name);
			if (!tournament.IsStarted())
				throw new DomainException(MatchServiceError.MatchCannotBeStartedInNotStartedTournament);
			if (tournament.StartedAt > startedAt)
				throw new DomainException(MatchServiceError.MatchCannotBeStartedBeforeTournamentStart, startedAt, tournament.StartedAt.Value);
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
				throw new DomainException(MatchServiceError.ForwardsCountOutOfRange, teamName, rules.MinForwards.ToString(), rules.MaxForwards.ToString());
			if (defensemanCount < rules.MinDefensemans || defensemanCount > rules.MaxDefensemans)
				throw new DomainException(MatchServiceError.DefensemenCountOutOfRange, teamName, rules.MinDefensemans.ToString(), rules.MaxDefensemans.ToString());
			if (goalieCount < rules.MinGoalies || goalieCount > rules.MaxGoalies)
				throw new DomainException(MatchServiceError.GoaliesCountOutOfRange, teamName, rules.MinGoalies.ToString(), rules.MaxGoalies.ToString());
			if (playersCount < rules.MinPlayers || playersCount > rules.MaxPlayers)
				throw new DomainException(MatchServiceError.PlayersCountOutOfRange, teamName, rules.MinPlayers.ToString(), rules.MaxPlayers.ToString());

		}
		private bool IsTeamInTournament(Tournament tournament, int teamId) => tournament.TeamsId.Contains(teamId);

	}
}
