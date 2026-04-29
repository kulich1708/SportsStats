using SportsStats.Domain.Matches;
using SportsStats.Domain.Players;
using SportsStats.Domain.Services;
using SportsStats.Domain.Teams;
using SportsStats.Domain.Tournaments;
using SportsStats.Domain.Tournaments.Rules;
using System;

namespace SportsStats.Tests.Domain.Services
{
	public class MatchServiceTests
	{
		private readonly MatchService _matchService = new();

		[Fact]
		public void CreateMatch_WhenTournamentIsRegistration_ReturnsMatch()
		{
			Tournament tournament = CreateTournamentInRegistration();
			DateTime scheduledAt = new(2026, 4, 29, 19, 0, 0);

			Match match = _matchService.CreateMatch(tournament, 10, 20, scheduledAt, tournament.TournamentRules!);

			Assert.NotNull(match);
			Assert.Equal(10, match.HomeTeamId);
			Assert.Equal(20, match.AwayTeamId);
			Assert.Equal(MatchStatus.Waiting, match.Status);
		}

		[Fact]
		public void CreateMatch_WhenTournamentIsInProgress_ReturnsMatch()
		{
			Tournament tournament = CreateTournamentInProgress();
			DateTime scheduledAt = tournament.StartedAt!.Value.AddHours(1);

			Match match = _matchService.CreateMatch(tournament, 10, 20, scheduledAt, tournament.TournamentRules!);

			Assert.NotNull(match);
			Assert.Equal(10, match.HomeTeamId);
			Assert.Equal(20, match.AwayTeamId);
		}

		[Fact]
		public void CreateMatch_WhenTournamentIsDraft_ThrowsArgumentException()
		{
			Tournament tournament = new("KHL Test");
			TournamentRules rules = TournamentRules.CreateKHLRules();
			DateTime scheduledAt = new(2026, 4, 29, 19, 0, 0);

			var ex = Assert.Throws<ArgumentException>(() => _matchService.CreateMatch(tournament, 10, 20, scheduledAt, rules));

			Assert.Contains("ещё не открыт", ex.Message, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public void CreateMatch_WhenHomeTeamNotInTournament_ThrowsArgumentException()
		{
			Tournament tournament = CreateTournamentInRegistration();
			DateTime scheduledAt = new(2026, 4, 29, 19, 0, 0);

			var ex = Assert.Throws<ArgumentException>(() => _matchService.CreateMatch(tournament, 999, 20, scheduledAt, tournament.TournamentRules!));

			Assert.Contains("Домашняя команда", ex.Message, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public void CreateMatch_WhenAwayTeamNotInTournament_ThrowsArgumentException()
		{
			Tournament tournament = CreateTournamentInRegistration();
			DateTime scheduledAt = new(2026, 4, 29, 19, 0, 0);

			var ex = Assert.Throws<ArgumentException>(() => _matchService.CreateMatch(tournament, 10, 999, scheduledAt, tournament.TournamentRules!));

			Assert.Contains("Гостевая команда", ex.Message, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public void CreateMatch_WhenScheduledAtEarlierThanTournamentStartedAt_ThrowsArgumentException()
		{
			Tournament tournament = CreateTournamentInProgress();
			DateTime scheduledAt = tournament.StartedAt!.Value.AddMinutes(-10);

			var ex = Assert.Throws<ArgumentException>(() => _matchService.CreateMatch(tournament, 10, 20, scheduledAt, tournament.TournamentRules!));

			Assert.Contains("был открыт только", ex.Message, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public void Start_WhenTournamentIsNotStarted_ThrowsArgumentException()
		{
			Tournament tournament = CreateTournamentInRegistration();
			Match match = CreateMatchForStart(tournament.TournamentRules!);
			List<Player> homeRoster = CreateValidRoster();
			List<Player> awayRoster = CreateValidRoster();
			Team homeTeam = new("Home Team");
			Team awayTeam = new("Away Team");
			DateTime startedAt = new(2026, 4, 29, 19, 30, 0);

			var ex = Assert.Throws<ArgumentException>(() => _matchService.Start(match, tournament, homeRoster, awayRoster, homeTeam, awayTeam, startedAt));

			Assert.Contains("неначатом турнире", ex.Message, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public void Start_WhenStartedAtEarlierThanTournamentStartedAt_ThrowsArgumentException()
		{
			Tournament tournament = CreateTournamentInProgress();
			Match match = CreateMatchForStart(tournament.TournamentRules!);
			List<Player> homeRoster = CreateValidRoster();
			List<Player> awayRoster = CreateValidRoster();
			Team homeTeam = new("Home Team");
			Team awayTeam = new("Away Team");
			DateTime startedAt = tournament.StartedAt!.Value.AddMinutes(-1);

			var ex = Assert.Throws<ArgumentException>(() => _matchService.Start(match, tournament, homeRoster, awayRoster, homeTeam, awayTeam, startedAt));

			Assert.Contains("данный турнир начался", ex.Message, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public void Start_WhenRosterViolatesRules_ThrowsArgumentException()
		{
			Tournament tournament = CreateTournamentInProgress();
			Match match = CreateMatchForStart(tournament.TournamentRules!);
			List<Player> invalidHomeRoster = CreateInvalidRosterWithoutGoalie();
			List<Player> awayRoster = CreateValidRoster();
			Team homeTeam = new("Home Team");
			Team awayTeam = new("Away Team");
			DateTime startedAt = tournament.StartedAt!.Value.AddMinutes(30);

			var ex = Assert.Throws<ArgumentException>(() => _matchService.Start(match, tournament, invalidHomeRoster, awayRoster, homeTeam, awayTeam, startedAt));

			Assert.Contains("Проверьте состав команды", ex.Message, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public void Start_WhenInputIsValid_StartsMatch()
		{
			Tournament tournament = CreateTournamentInProgress();
			Match match = CreateMatchForStart(tournament.TournamentRules!);
			List<Player> homeRoster = CreateValidRoster();
			List<Player> awayRoster = CreateValidRoster();
			Team homeTeam = new("Home Team");
			Team awayTeam = new("Away Team");
			DateTime startedAt = tournament.StartedAt!.Value.AddMinutes(30);

			_matchService.Start(match, tournament, homeRoster, awayRoster, homeTeam, awayTeam, startedAt);

			Assert.True(match.IsMatchInProgress());
			Assert.Equal(startedAt, match.StartedAt);
		}

		private static Match CreateMatchForStart(TournamentRules rules)
		{
			DateTime scheduledAt = new(2026, 4, 29, 19, 0, 0);
			Match match = new(1, 10, 20, rules, scheduledAt);
			return match;
		}

		private static Tournament CreateTournamentInRegistration()
		{
			Tournament tournament = new("KHL Test");
			tournament.SetRules(TournamentRules.CreateKHLRules());
			tournament.Registration();
			tournament.RegistrateTeam(10);
			tournament.RegistrateTeam(20);
			return tournament;
		}

		private static Tournament CreateTournamentInProgress()
		{
			Tournament tournament = CreateTournamentInRegistration();
			tournament.Start(new DateTime(2026, 4, 29, 19, 0, 0));
			return tournament;
		}

		private static List<Player> CreateValidRoster()
		{
			List<Player> players = new();
			players.AddRange(CreatePlayers(PositionType.LeftWinger, 4, "LW"));
			players.AddRange(CreatePlayers(PositionType.RightWinger, 4, "RW"));
			players.AddRange(CreatePlayers(PositionType.Center, 4, "C"));
			players.AddRange(CreatePlayers(PositionType.LeftDefenseman, 3, "LD"));
			players.AddRange(CreatePlayers(PositionType.RightDefenseman, 3, "RD"));
			players.AddRange(CreatePlayers(PositionType.Goalie, 2, "G"));
			return players;
		}

		private static List<Player> CreateInvalidRosterWithoutGoalie()
		{
			List<Player> players = new();
			players.AddRange(CreatePlayers(PositionType.LeftWinger, 4, "LW"));
			players.AddRange(CreatePlayers(PositionType.RightWinger, 4, "RW"));
			players.AddRange(CreatePlayers(PositionType.Center, 4, "C"));
			players.AddRange(CreatePlayers(PositionType.LeftDefenseman, 4, "LD"));
			players.AddRange(CreatePlayers(PositionType.RightDefenseman, 4, "RD"));
			return players;
		}

		private static List<Player> CreatePlayers(PositionType position, int count, string prefix)
		{
			List<Player> players = new();
			for (int i = 1; i <= count; i++)
			{
				Player player = new($"{prefix}{i}", $"Test{i}", position);
				players.Add(player);
			}
			return players;
		}
	}
}
