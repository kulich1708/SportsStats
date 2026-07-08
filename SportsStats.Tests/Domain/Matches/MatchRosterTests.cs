using SportsStats.Domain.Matches;
using SportsStats.Domain.Matches.Goals;
using SportsStats.Domain.Shared;
using SportsStats.Domain.Tournaments.Rules;
using System;

namespace SportsStats.Tests.Domain.Matches
{
	public class MatchRosterTests
	{
		[Fact]
		public void AddPlayerToRoster_WhenTeamNotInMatch_ThrowsArgumentException()
		{
			Match match = CreateMatch();

			var ex = Assert.Throws<DomainException>(() => match.AddPlayerToRoster(playerId: 101, teamId: 999));

			Assert.Equal(MatchError.TeamNotInMatch.Code, ex.Code);
		}

		[Fact]
		public void AddPlayerToRoster_WhenPlayerAlreadyOnRoster_ThrowsArgumentException()
		{
			Match match = CreateMatch();
			match.AddPlayerToRoster(playerId: 101, teamId: 10);

			var ex = Assert.Throws<DomainException>(() => match.AddPlayerToRoster(playerId: 101, teamId: 10));

			Assert.Equal(MatchError.PlayerAlreadyAdded.Code, ex.Code);
		}

		[Fact]
		public void AddPlayerToRoster_WhenMatchAlreadyStarted_ThrowsArgumentException()
		{
			Match match = CreateMatch();
			match.Start(new DateTime(2026, 4, 29, 19, 30, 0));

			var ex = Assert.Throws<DomainException>(() => match.AddPlayerToRoster(playerId: 101, teamId: 10));

			Assert.Equal(MatchError.CannotAddPlayerAfterMatchStart.Code, ex.Code);
		}

		[Fact]
		public void AddPlayerToRoster_WhenValid_AddsPlayerToCorrectRoster()
		{
			Match match = CreateMatch();

			match.AddPlayerToRoster(playerId: 101, teamId: 10);

			Assert.Contains(101, match.HomeTeam.Roster);
			Assert.DoesNotContain(101, match.AwayTeam.Roster);
		}

		private static Match CreateMatch()
		{
			TournamentRules rules = TournamentRules.CreateKHLRules();
			DateTime scheduledAt = new(2026, 4, 29, 19, 0, 0);
			Match match = new(1, 10, 20, rules, scheduledAt);
			return match;
		}
	}
}
