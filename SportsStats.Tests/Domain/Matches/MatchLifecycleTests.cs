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
			Assert.Equal(10, match.HomeTeam.Id);
			Assert.Equal(20, match.AwayTeam.Id);
			Assert.Equal(scheduleAt, match.ScheduledAt);
			Assert.Equal(MatchStatus.Waiting, match.Status);
			Assert.Equal(0, match.HomeTeam.Score);
			Assert.Equal(0, match.AwayTeam.Score);
			Assert.Empty(match.Goals);
			Assert.Empty(match.HomeTeam.Roster);
			Assert.Empty(match.AwayTeam.Roster);
			Assert.Equal(0, match.Period.Current);
			Assert.True(match.Period.IsBreak);
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
			Assert.Equal(1, match.Period.Current);
			Assert.False(match.Period.IsBreak);
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
		public void FinishPeriod_WhenPeriodStarted_ChangesPeriod()
		{
			Match match = CreateMatch(TournamentRules.CreateKHLRules());
			DateTime startedAt = new(2026, 4, 29, 19, 30, 0);
			match.Start(startedAt);

			match.FinishPeriod(DateTime.UtcNow);

			Assert.Equal(1, match.Period.Current);
			Assert.True(match.Period.IsBreak);
		}
		[Fact]
		public void FinishPeriod_WhenMatchNotStarted_ThrowsArgumentException()
		{
			Match match = CreateMatch(TournamentRules.CreateKHLRules());
			var ex = Assert.Throws<ArgumentException>(() => match.FinishPeriod(DateTime.UtcNow));

			Assert.Contains("Матч не начат или закончен", ex.Message, StringComparison.OrdinalIgnoreCase);
		}
		[Fact]
		public void FinishPeriod_WhenPeriodFinished_ThrowsArgumentException()
		{
			Match match = CreateMatch(TournamentRules.CreateKHLRules());
			DateTime startedAt = new(2026, 4, 29, 19, 30, 0);
			match.Start(startedAt);

			match.FinishPeriod(DateTime.UtcNow);

			var ex = Assert.Throws<ArgumentException>(() => match.FinishPeriod(DateTime.UtcNow));

			Assert.Contains("Период уже закончен", ex.Message, StringComparison.OrdinalIgnoreCase);
		}


		[Fact]
		public void StartPeriod_WhenPeriodFinished_ChangesPeriod()
		{
			Match match = CreateMatch(TournamentRules.CreateKHLRules());
			DateTime startedAt = new(2026, 4, 29, 19, 30, 0);
			match.Start(startedAt);

			match.FinishPeriod(DateTime.UtcNow);
			match.StartPeriod();

			Assert.Equal(2, match.Period.Current);
			Assert.False(match.Period.IsBreak);
		}
		[Fact]
		public void StartPeriod_WhenMatchNotStarted_ThrowsArgumentException()
		{
			Match match = CreateMatch(TournamentRules.CreateKHLRules());
			var ex = Assert.Throws<ArgumentException>(() => match.StartPeriod());

			Assert.Contains("Матч не начат или закончен", ex.Message, StringComparison.OrdinalIgnoreCase);
		}
		[Fact]
		public void StartPeriod_WhenPeriodStarted_ThrowsArgumentException()
		{
			Match match = CreateMatch(TournamentRules.CreateKHLRules());
			DateTime startedAt = new(2026, 4, 29, 19, 30, 0);
			match.Start(startedAt);

			match.FinishPeriod(DateTime.UtcNow);
			match.StartPeriod();
			var ex = Assert.Throws<ArgumentException>(() => match.StartPeriod());

			Assert.Contains("Период уже начат", ex.Message, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public void FinishPeriod_WhenHomeWinsInRegulation_FinishMatchAndSetsWinTypes()
		{
			Match match = CreateMatch(CreateDrawNotAllowedRules());
			PrepareRosters(match);
			DateTime startedAt = new(2026, 4, 29, 19, 30, 0);
			match.Start(startedAt);
			match.AddGoal(10, 101, 300, startedAt.AddMinutes(5));

			match.FinishPeriod(DateTime.UtcNow);
			match.StartPeriod();
			match.FinishPeriod(DateTime.UtcNow);
			match.StartPeriod();
			match.FinishPeriod(DateTime.UtcNow);

			Assert.Equal(MatchStatus.Finished, match.Status);
			Assert.Equal(MatchWinType.REGULATION_WIN, match.HomeTeam.WinType);
			Assert.Equal(MatchWinType.REGULATION_LOSS, match.AwayTeam.WinType);
		}

		[Fact]
		public void FinishPeriod_WhenAwayWinsInOvertime_FinishMatchAndSetsWinTypes()
		{
			Match match = CreateMatch(CreateDrawNotAllowedRules());
			PrepareRosters(match);
			DateTime startedAt = new(2026, 4, 29, 19, 30, 0);
			DateTime finishedAt = new(2026, 4, 29, 20, 40, 0);
			match.Start(startedAt);

			match.AddGoal(10, 101, 300, startedAt.AddMinutes(5));

			match.FinishPeriod(DateTime.UtcNow);
			match.StartPeriod();

			match.AddGoal(20, 201, 600, startedAt.AddMinutes(25));

			match.FinishPeriod(DateTime.UtcNow);
			match.StartPeriod();
			match.FinishPeriod(DateTime.UtcNow);
			match.StartPeriod();

			match.AddGoal(20, 201, 30, startedAt.AddMinutes(61));

			Assert.Equal(MatchStatus.Finished, match.Status);
			Assert.True(match.IsOvertime);
			Assert.Equal(MatchWinType.OT_LOSS, match.HomeTeam.WinType);
			Assert.Equal(MatchWinType.OT_WIN, match.AwayTeam.WinType);
		}
		[Fact]
		public void FinishPeriod_WhenScoreEqualInOvertime_FinishMatchAndSetsWinTypes()
		{
			Match match = CreateMatch(CreateRulesForTwoOvertimes());
			PrepareRosters(match);
			DateTime startedAt = new(2026, 4, 29, 19, 30, 0);
			DateTime finishedAt = new(2026, 4, 29, 20, 40, 0);
			match.Start(startedAt);

			match.AddGoal(10, 101, 5 * 60, startedAt.AddMinutes(5));

			match.FinishPeriod(DateTime.UtcNow);
			match.StartPeriod();

			match.AddGoal(20, 201, 10 * 60, startedAt.AddMinutes(25));

			match.FinishPeriod(DateTime.UtcNow);
			match.StartPeriod();
			match.FinishPeriod(DateTime.UtcNow);
			match.StartPeriod();

			match.AddGoal(20, 201, 2 * 60, startedAt.AddMinutes(61));
			match.FinishPeriod(DateTime.UtcNow);
			match.StartPeriod();
			match.AddGoal(10, 101, 4 * 60, startedAt.AddMinutes(68));
			match.FinishPeriod(DateTime.UtcNow);

			Assert.Equal(MatchStatus.Finished, match.Status);
			Assert.True(match.IsOvertime);
			Assert.Equal(MatchWinType.DRAW, match.HomeTeam.WinType);
			Assert.Equal(MatchWinType.DRAW, match.AwayTeam.WinType);
		}
		[Fact]
		public void FinishPeriod_WhenScoreEqualInOneInfinityOvertime_ThrowsArgumentException()
		{
			Match match = CreateMatch(CreateRulesForOneInfinityOvertime());
			PrepareRosters(match);
			DateTime startedAt = new(2026, 4, 29, 19, 30, 0);
			DateTime finishedAt = new(2026, 4, 29, 20, 40, 0);
			match.Start(startedAt);
			match.FinishPeriod(DateTime.UtcNow);
			match.StartPeriod();
			match.FinishPeriod(DateTime.UtcNow);
			match.StartPeriod();
			match.FinishPeriod(DateTime.UtcNow);
			match.StartPeriod();

			var ex = Assert.Throws<ArgumentException>(() => match.FinishPeriod(DateTime.UtcNow));

			Assert.Contains("Нельзя завершить бесконечный", ex.Message);
		}
		[Fact]
		public void StartAndFinishPeriods_WhenThreePeriodsAndOneOvertime_CorrectPeriodTitle()
		{
			Match match = CreateMatch(CreateRulesForOneInfinityOvertime());
			PrepareRosters(match);
			DateTime startedAt = new(2026, 4, 29, 19, 30, 0);
			DateTime finishedAt = new(2026, 4, 29, 20, 40, 0);
			match.Start(startedAt);
			Assert.Equal("Завершить период 1", match.Period.Title);
			match.FinishPeriod(DateTime.UtcNow);
			Assert.Equal("Начать период 2", match.Period.Title);
			match.StartPeriod();
			Assert.Equal("Завершить период 2", match.Period.Title);
			match.FinishPeriod(DateTime.UtcNow);
			Assert.Equal("Начать период 3", match.Period.Title);
			match.StartPeriod();
			Assert.Equal("Завершить период 3", match.Period.Title);
			match.FinishPeriod(DateTime.UtcNow);
			Assert.Equal("Начать овертайм", match.Period.Title);
			match.StartPeriod();
			Assert.Equal(null, match.Period.Title);
		}
		[Fact]
		public void StartAndFinishPeriods_WhenTwoOvertimesAndDraw_CorrectPeriodTitle()
		{
			Match match = CreateMatch(CreateRulesForTwoOvertimes());
			PrepareRosters(match);
			DateTime startedAt = new(2026, 4, 29, 19, 30, 0);
			DateTime finishedAt = new(2026, 4, 29, 20, 40, 0);
			match.Start(startedAt);
			match.FinishPeriod(DateTime.UtcNow);
			match.StartPeriod();
			match.FinishPeriod(DateTime.UtcNow);
			match.StartPeriod();
			match.FinishPeriod(DateTime.UtcNow);
			Assert.Equal("Начать овертайм 1", match.Period.Title);
			match.StartPeriod();
			Assert.Equal("Завершить овертайм 1", match.Period.Title);
			match.FinishPeriod(DateTime.UtcNow);
			Assert.Equal("Начать овертайм 2", match.Period.Title);
			match.StartPeriod();
			Assert.Equal("Завершить овертайм 2", match.Period.Title);
			match.FinishPeriod(DateTime.UtcNow);
			Assert.Equal(null, match.Period.Title);
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
		private static TournamentRules CreateRulesForTwoOvertimes()
		{
			MatchOvertimeRules overtimeRules = new(overtimesCount: 2, overtimeDurationSeconds: 300, goalEndsOvertime: false);
			MatchTimeRules timeRules = new(
				periodsCount: 3,
				periodDurationSeconds: 20 * 60,
				isDrawPossible: true,
				hasOvertime: true,
				hasShootout: false,
				overtimeRules: overtimeRules);
			MatchRosterRules rosterRules = MatchRosterRules.CreateKHLRules();
			MatchPointsRules pointsRules = new(winPoints: 2, lossPoints: 0, otWinPoints: 2, otLossPoints: 1, drawPoints: 1);
			TournamentRules rules = new(timeRules, rosterRules, pointsRules);
			return rules;
		}
		private static TournamentRules CreateRulesForOneInfinityOvertime()
		{
			MatchOvertimeRules overtimeRules = new(overtimesCount: 1, overtimeDurationSeconds: null, goalEndsOvertime: true);
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
