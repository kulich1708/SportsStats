using SportsStats.Domain.Matches;
using SportsStats.Domain.Matches.Goals;
using SportsStats.Domain.Tournaments.Rules;
using SportsStats.Domain.Tournaments.Rules.MatchTime;
using System;

namespace SportsStats.Tests.Domain.Matches
{
	public class MatchGoalTests
	{
		[Fact]
		public void AddGoal_WhenMatchNotInProgress_ThrowsArgumentException()
		{
			Match match = CreateMatch(CreateRulesWithOvertime());
			PrepareRosters(match);

			var ex = Assert.Throws<ArgumentException>(() => match.AddGoal(10, 101, 1, 120, DateTime.UtcNow));

			Assert.Contains("сейчас не идёт", ex.Message, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public void AddGoal_WhenScoringTeamNotInMatch_ThrowsArgumentException()
		{
			Match match = CreateMatch(CreateRulesWithOvertime());
			PrepareRosters(match);
			match.Start(new DateTime(2026, 4, 29, 19, 30, 0));

			var ex = Assert.Throws<ArgumentException>(() => match.AddGoal(999, 101, 1, 120, DateTime.UtcNow));

			Assert.Contains("не учавствует", ex.Message, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public void AddGoal_WhenScorerNotInScoringTeamRoster_ThrowsArgumentException()
		{
			Match match = CreateMatch(CreateRulesWithOvertime());
			PrepareRosters(match);
			match.Start(new DateTime(2026, 4, 29, 19, 30, 0));

			var ex = Assert.Throws<ArgumentException>(() => match.AddGoal(10, 999, 1, 120, DateTime.UtcNow));

			Assert.Contains("нет в заявке", ex.Message, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public void AddGoal_WhenValid_IncrementsTeamScore()
		{
			Match match = CreateMatch(CreateRulesWithOvertime());
			PrepareRosters(match);
			match.Start(new DateTime(2026, 4, 29, 19, 30, 0));

			match.AddGoal(10, 101, 1, 120, DateTime.UtcNow);

			Assert.Equal(1, match.HomeTeamScore);
			Assert.Equal(0, match.AwayTeamScore);
			Assert.Single(match.Goals);
		}

		[Fact]
		public void AddGoal_InOvertimeWithGoalEndsOvertime_MarksGoalAsWinning()
		{
			Match match = CreateMatch(CreateRulesWithOvertime());
			PrepareRosters(match);
			match.Start(new DateTime(2026, 4, 29, 19, 30, 0));
			match.AddGoal(10, 101, 1, 120, DateTime.UtcNow);
			match.AddGoal(20, 201, 2, 120, DateTime.UtcNow);

			GoalEvent goal = match.AddGoal(10, 101, 4, 30, DateTime.UtcNow);

			Assert.True(goal.IsWinning);
			Assert.True(match.IsOvertime);
		}

		[Fact]
		public void AddGoal_WhenPeriodIsInvalid_ThrowsArgumentException()
		{
			Match match = CreateMatch(CreateRulesWithOvertime());
			PrepareRosters(match);
			match.Start(new DateTime(2026, 4, 29, 19, 30, 0));

			var ex = Assert.Throws<ArgumentException>(() => match.AddGoal(10, 101, 7, 120, DateTime.UtcNow));

			Assert.Contains("периода", ex.Message, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public void FillGoalDetails_WhenAssistIsGoalScorer_ThrowsArgumentException()
		{
			Match match = CreateMatch(CreateRulesWithOvertime());
			PrepareRosters(match);
			match.AddPlayerToRoster(102, 10);
			match.Start(new DateTime(2026, 4, 29, 19, 30, 0));
			GoalEvent goal = match.AddGoal(10, 101, 1, 120, DateTime.UtcNow);

			var ex = Assert.Throws<ArgumentException>(() => match.FillGoalDetails(
				goalId: goal.Id,
				goalScorerId: 101,
				firstAssistId: 101,
				secondAssistId: null,
				strengthType: GoalStrengthType.EvenStrength,
				netType: GoalNetType.EmptyNet));

			Assert.Contains("ассистентом", ex.Message, StringComparison.OrdinalIgnoreCase);
		}

		private static Match CreateMatch(TournamentRules rules)
		{
			DateTime scheduledAt = new(2026, 4, 29, 19, 0, 0);
			Match match = new(1, 10, 20, rules, scheduledAt);
			return match;
		}

		private static TournamentRules CreateRulesWithOvertime()
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
