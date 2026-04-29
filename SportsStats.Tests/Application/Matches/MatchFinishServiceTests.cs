using Moq;
using SportsStats.Application.Matches;
using SportsStats.Domain.Matches;
using SportsStats.Domain.Shared;
using SportsStats.Domain.Statistics;
using SportsStats.Domain.Tournaments.Rules;
using SportsStats.Domain.Tournaments.Rules.MatchTime;
using System;
using MatchEntity = SportsStats.Domain.Matches.Match;

namespace SportsStats.Tests.Application.Matches
{
	public class MatchFinishServiceTests
	{
		private readonly Mock<IMatchRepository> _matchRepositoryMock = new();
		private readonly Mock<ITeamStatsRepository> _teamStatsRepositoryMock = new();
		private readonly Mock<ITimeProvider> _timeProviderMock = new();

		[Fact]
		public async Task FinishAsync_WhenMatchNotFound_ThrowsArgumentException()
		{
			MatchFinishService service = CreateService();
			_matchRepositoryMock.Setup(x => x.GetAsync(99)).ReturnsAsync((MatchEntity?)null);

			var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.FinishAsync(99));

			Assert.Contains("не существует", ex.Message, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public async Task FinishAsync_WhenFinishedAtNotProvided_UsesTimeProviderAndSavesMatch()
		{
			MatchFinishService service = CreateService();
			MatchEntity match = CreateStartedMatchWithHomeRegulationWin();
			DateTime finishedAt = new(2026, 4, 29, 20, 30, 0);
			TeamStats homeStats = new(match.HomeTeamId, match.TournamentId);
			TeamStats awayStats = new(match.AwayTeamId, match.TournamentId);

			_matchRepositoryMock.Setup(x => x.GetAsync(match.Id)).ReturnsAsync(match);
			_timeProviderMock.Setup(x => x.GetCurrentTime()).Returns(finishedAt);
			_teamStatsRepositoryMock.Setup(x => x.GetAsync(match.HomeTeamId, match.TournamentId)).ReturnsAsync(homeStats);
			_teamStatsRepositoryMock.Setup(x => x.GetAsync(match.AwayTeamId, match.TournamentId)).ReturnsAsync(awayStats);

			await service.FinishAsync(match.Id);

			Assert.True(match.IsMatchFinished());
			Assert.Equal(finishedAt, match.FinishedAt);
			_timeProviderMock.Verify(x => x.GetCurrentTime(), Times.Once);
			_matchRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
		}

		[Fact]
		public async Task FinishAsync_WhenOutcomePointsConfigured_UpdatesBothTeamStats()
		{
			MatchFinishService service = CreateService();
			MatchEntity match = CreateStartedMatchWithHomeRegulationWin();
			DateTime finishedAt = new(2026, 4, 29, 20, 30, 0);
			TeamStats homeStats = new(match.HomeTeamId, match.TournamentId);
			TeamStats awayStats = new(match.AwayTeamId, match.TournamentId);

			_matchRepositoryMock.Setup(x => x.GetAsync(match.Id)).ReturnsAsync(match);
			_teamStatsRepositoryMock.Setup(x => x.GetAsync(match.HomeTeamId, match.TournamentId)).ReturnsAsync(homeStats);
			_teamStatsRepositoryMock.Setup(x => x.GetAsync(match.AwayTeamId, match.TournamentId)).ReturnsAsync(awayStats);

			await service.FinishAsync(match.Id, finishedAt);

			Assert.Equal(1, homeStats.Games);
			Assert.Equal(1, homeStats.RegularWins);
			Assert.Equal(2, homeStats.Points);
			Assert.Equal(1, awayStats.Games);
			Assert.Equal(1, awayStats.RegularLosses);
			Assert.Equal(0, awayStats.Points);
			_teamStatsRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
		}

		[Fact]
		public async Task FinishAsync_WhenPointsForOutcomeMissing_ThrowsArgumentException()
		{
			MatchFinishService service = CreateService();
			MatchEntity match = CreateStartedMatchWithDrawAndMissingDrawPoints();
			DateTime finishedAt = new(2026, 4, 29, 20, 30, 0);
			TeamStats homeStats = new(match.HomeTeamId, match.TournamentId);
			TeamStats awayStats = new(match.AwayTeamId, match.TournamentId);

			_matchRepositoryMock.Setup(x => x.GetAsync(match.Id)).ReturnsAsync(match);
			_teamStatsRepositoryMock.Setup(x => x.GetAsync(match.HomeTeamId, match.TournamentId)).ReturnsAsync(homeStats);
			_teamStatsRepositoryMock.Setup(x => x.GetAsync(match.AwayTeamId, match.TournamentId)).ReturnsAsync(awayStats);

			var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.FinishAsync(match.Id, finishedAt));

			Assert.Contains("не установлнено количество очков", ex.Message, StringComparison.OrdinalIgnoreCase);
			_teamStatsRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Never);
		}

		private MatchFinishService CreateService()
		{
			return new MatchFinishService(
				_teamStatsRepositoryMock.Object,
				_matchRepositoryMock.Object,
				_timeProviderMock.Object);
		}

		private static MatchEntity CreateStartedMatchWithHomeRegulationWin()
		{
			TournamentRules rules = CreateRulesForRegulationWin();
			DateTime scheduledAt = new(2026, 4, 29, 19, 0, 0);
			DateTime startedAt = new(2026, 4, 29, 19, 30, 0);
			MatchEntity match = new(1, 10, 20, rules, scheduledAt);
			match.SetIdForTest(777);
			match.AddPlayerToRoster(101, 10);
			match.AddPlayerToRoster(201, 20);
			match.Start(startedAt);
			match.AddGoal(10, 101, 1, 120, startedAt.AddMinutes(2));
			return match;
		}

		private static MatchEntity CreateStartedMatchWithDrawAndMissingDrawPoints()
		{
			TournamentRules rules = CreateRulesWithDrawButWithoutDrawPoints();
			DateTime scheduledAt = new(2026, 4, 29, 19, 0, 0);
			DateTime startedAt = new(2026, 4, 29, 19, 30, 0);
			MatchEntity match = new(1, 10, 20, rules, scheduledAt);
			match.SetIdForTest(888);
			match.Start(startedAt);
			return match;
		}

		private static TournamentRules CreateRulesForRegulationWin()
		{
			MatchTimeRules timeRules = new(
				periodsCount: 3,
				periodDurationSeconds: 20 * 60,
				isDrawPossible: false,
				hasOvertime: true,
				hasShootout: false,
				overtimeRules: new MatchOvertimeRules(overtimesCount: 1, overtimeDurationSeconds: 300, goalEndsOvertime: true));
			MatchRosterRules rosterRules = MatchRosterRules.CreateKHLRules();
			MatchPointsRules pointsRules = new(winPoints: 2, lossPoints: 0, otWinPoints: 2, otLossPoints: 1);
			return new TournamentRules(timeRules, rosterRules, pointsRules);
		}

		private static TournamentRules CreateRulesWithDrawButWithoutDrawPoints()
		{
			MatchTimeRules timeRules = new(
				periodsCount: 3,
				periodDurationSeconds: 20 * 60,
				isDrawPossible: true,
				hasOvertime: false,
				hasShootout: false);
			MatchRosterRules rosterRules = MatchRosterRules.CreateKHLRules();
			MatchPointsRules pointsRules = new(winPoints: 2, lossPoints: 0, drawPoints: 1);
			TournamentRules rules = new(timeRules, rosterRules, pointsRules);
			typeof(MatchPointsRules).GetProperty("DrawPoints")!.SetValue(rules.MatchPointsRules, null);
			return rules;
		}
	}

	internal static class TestEntityExtensions
	{
		public static void SetIdForTest(this SportsStats.Domain.Common.BaseEntity entity, int id)
		{
			typeof(SportsStats.Domain.Common.BaseEntity)
				.GetProperty("Id")!
				.SetValue(entity, id);
		}
	}
}
