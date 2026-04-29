using SportsStats.Domain.Matches;
using SportsStats.Domain.Tournaments.Rules;
using SportsStats.Domain.Tournaments.Rules.MatchTime;
using SportsStats.Domain.Shared.Enums;
using System;

namespace SportsStats.Tests.Domain.Matches
{
	public class MatchLifecycleTests
	{
		[Fact]
		public void Constructor_WhenHomeAndAwayTeamsAreDifferent_CreatesMatch()
		{
			var rules = TournamentRules.CreateKHLRules();
			DateTime scheduleAt = new(2026, 4, 29, 19, 30, 0);

			Match match = new(1, 10, 20, rules, scheduleAt);

			Assert.NotNull(match);
			Assert.Equal(1, match.TournamentId);
			Assert.Equal(10, match.HomeTeamId);
			Assert.Equal(20, match.AwayTeamId);
			Assert.Equal(scheduleAt, match.ScheduledAt);
			Assert.Equal(MatchStatus.Waiting, match.Status);
			Assert.Equal(0, match.HomeTeamScore);
			Assert.Equal(0, match.AwayTeamScore);
			Assert.Empty(match.Goals);
			Assert.Empty(match.HomeTeamRoster);
			Assert.Empty(match.AwayTeamRoster);
		}
		[Fact]
		public void Constructor_WhenHomeAndAwayTeamsAreSame_ThrowsArgumentException()
		{
			var rules = TournamentRules.CreateKHLRules();
			DateTime scheduleAt = new(2026, 4, 29, 19, 30, 0);

			var ex = Assert.Throws<ArgumentException>(() => new Match(1, 1, 1, rules, scheduleAt));

			Assert.Contains("собой", ex.Message, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public void Start_WhenWaiting_ChangesStatusToInProgress()
		{
			Match match = CreateMatch(TournamentRules.CreateKHLRules());
			DateTime startedAt = new(2026, 4, 29, 19, 30, 0);

			match.Start(startedAt);

			Assert.True(match.IsMatchInProgress());
			Assert.Equal(MatchStatus.InProgress, match.Status);
			Assert.Equal(startedAt, match.StartedAt);
		}

		[Fact]
		public void Start_WhenNotWaiting_ThrowsArgumentException()
		{
			Match match = CreateMatch(TournamentRules.CreateKHLRules());
			DateTime startedAt = new(2026, 4, 29, 19, 30, 0);
			match.Start(startedAt);

			var ex = Assert.Throws<ArgumentException>(() => match.Start(startedAt.AddMinutes(5)));

			Assert.Contains("ожидании", ex.Message, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public void Finish_WhenInProgress_ChangesStatusToFinished()
		{
			Match match = CreateMatch(CreateDrawNotAllowedRules());
			PrepareRosters(match);
			DateTime startedAt = new(2026, 4, 29, 19, 30, 0);
			DateTime finishedAt = new(2026, 4, 29, 20, 30, 0);
			match.Start(startedAt);
			match.AddGoal(10, 101, 1, 300, startedAt.AddMinutes(5));

			match.Finish(finishedAt);

			Assert.True(match.IsMatchFinished());
			Assert.Equal(MatchStatus.Finished, match.Status);
			Assert.Equal(finishedAt, match.FinishedAt);
		}

		[Fact]
		public void Finish_WhenNotInProgress_ThrowsArgumentException()
		{
			Match match = CreateMatch(TournamentRules.CreateKHLRules());
			DateTime finishedAt = new(2026, 4, 29, 20, 30, 0);

			var ex = Assert.Throws<ArgumentException>(() => match.Finish(finishedAt));

			Assert.Contains("ещё не начат", ex.Message, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public void Finish_WhenScoreIsDrawAndDrawNotAllowed_ThrowsArgumentException()
		{
			Match match = CreateMatch(CreateDrawNotAllowedRules());
			DateTime startedAt = new(2026, 4, 29, 19, 30, 0);
			DateTime finishedAt = new(2026, 4, 29, 20, 30, 0);
			match.Start(startedAt);

			var ex = Assert.Throws<ArgumentException>(() => match.Finish(finishedAt));

			Assert.Contains("ничейным", ex.Message, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public void Finish_WhenHomeWinsInRegulation_SetsWinTypes()
		{
			Match match = CreateMatch(CreateDrawNotAllowedRules());
			PrepareRosters(match);
			DateTime startedAt = new(2026, 4, 29, 19, 30, 0);
			DateTime finishedAt = new(2026, 4, 29, 20, 30, 0);
			match.Start(startedAt);
			match.AddGoal(10, 101, 1, 300, startedAt.AddMinutes(5));

			match.Finish(finishedAt);

			Assert.Equal(MatchWinType.REGULATION_WIN, match.HomeTeamWinType);
			Assert.Equal(MatchWinType.REGULATION_LOSS, match.AwayTeamWinType);
		}

		[Fact]
		public void Finish_WhenAwayWinsInOvertime_SetsWinTypes()
		{
			Match match = CreateMatch(CreateDrawNotAllowedRules());
			PrepareRosters(match);
			DateTime startedAt = new(2026, 4, 29, 19, 30, 0);
			DateTime finishedAt = new(2026, 4, 29, 20, 40, 0);
			match.Start(startedAt);

			match.AddGoal(10, 101, 1, 300, startedAt.AddMinutes(5));
			match.AddGoal(20, 201, 2, 600, startedAt.AddMinutes(25));
			match.AddGoal(20, 201, 4, 30, startedAt.AddMinutes(61));

			match.Finish(finishedAt);

			Assert.Equal(MatchWinType.OT_LOSS, match.HomeTeamWinType);
			Assert.Equal(MatchWinType.OT_WIN, match.AwayTeamWinType);
		}

		private static Match CreateMatch(TournamentRules rules)
		{
			DateTime scheduledAt = new(2026, 4, 29, 19, 0, 0);
			Match match = new(1, 10, 20, rules, scheduledAt);
			return match;
		}

		private static TournamentRules CreateDrawNotAllowedRules()
		{
			MatchOvertimeRules overtimeRules = new(overtimesCount: 1, overtimeDurationSeconds: 300, goalEndsOvertime: true);
			MatchTimeRules timeRules = new(
				periodsCount: 3,
				periodDurationSeconds: 20 * 60,
				isDrawPossible: false,
				hasOvertime: true,
				hasShootout: false,
				overtimeRules: overtimeRules);
			MatchRosterRules rosterRules = MatchRosterRules.CreateKHLRules();
			MatchPointsRules pointsRules = new(winPoints: 2, lossPoints: 0, otWinPoints: 2, otLossPoints: 1);
			TournamentRules rules = new(timeRules, rosterRules, pointsRules);
			return rules;
		}

		private static void PrepareRosters(Match match)
		{
			match.AddPlayerToRoster(101, 10);
			match.AddPlayerToRoster(201, 20);
		}
	}
}
