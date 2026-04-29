using SportsStats.Domain.Matches;
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

			var ex = Assert.Throws<ArgumentException>(() => match.AddPlayerToRoster(playerId: 101, teamId: 999));

			Assert.Contains("Команда не учавствует", ex.Message, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public void AddPlayerToRoster_WhenPlayerAlreadyOnRoster_ThrowsArgumentException()
		{
			Match match = CreateMatch();
			match.AddPlayerToRoster(playerId: 101, teamId: 10);

			var ex = Assert.Throws<ArgumentException>(() => match.AddPlayerToRoster(playerId: 101, teamId: 10));

			Assert.Contains("дважды", ex.Message, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public void AddPlayerToRoster_WhenMatchAlreadyStarted_ThrowsArgumentException()
		{
			Match match = CreateMatch();
			match.Start(new DateTime(2026, 4, 29, 19, 30, 0));

			var ex = Assert.Throws<ArgumentException>(() => match.AddPlayerToRoster(playerId: 101, teamId: 10));

			Assert.Contains("после начала", ex.Message, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public void AddPlayerToRoster_WhenValid_AddsPlayerToCorrectRoster()
		{
			Match match = CreateMatch();

			match.AddPlayerToRoster(playerId: 101, teamId: 10);

			Assert.Contains(101, match.HomeTeamRoster);
			Assert.DoesNotContain(101, match.AwayTeamRoster);
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
