using ConsoleApp;
using Microsoft.EntityFrameworkCore;
using SportsStats.Application.Matches;
using SportsStats.Application.Players;
using SportsStats.Application.Players.DTOs.Responses;
using SportsStats.Application.Teams;
using SportsStats.Application.Teams.DTOs.Responses;
using SportsStats.Application.Tournaments;
using SportsStats.Domain.Players;
using SportsStats.Domain.Services;
using SportsStats.Infrastructure.Persistence.DbContexts;
using SportsStats.Infrastructure.Persistence.Repositories;
using SportsStats.Infrastructure.Services;
using System.Collections.Immutable;
using System.Text;
using System.Text.Json;

namespace SportsStats.ConsoleApp
{
	public class DataGenerator
	{
		private readonly SystemTimeProvider _timeProvider = new SystemTimeProvider();
		private readonly Random _random = new();
		private readonly DbContextOptionsBuilder<AppDbContext> _options;
		private readonly AppDbContext _context;
		private readonly PlayerRepository _playerRepository;
		private readonly TeamRepository _teamRepository;
		private readonly TournamentRepository _tournamentRepository;
		private readonly MatchRepository _matchRepository;
		private readonly TeamStatsRepository _teamStatsRepository;

		private readonly MatchService _matchService;

		private readonly PlayerApplicationService _playerApplicationService;
		private readonly TeamApplicationService _teamApplicationService;
		private readonly TournamentApplicationService _tournamentApplicationService;

		private readonly MatchGoalService _matchGoalService;
		private readonly MatchLifecycleService _matchLifecycleService;
		private readonly MatchFinishService _matchFinishService;
		private readonly MatchRosterService _matchRosterService;
		private readonly MatchQueriesHandler _matchQueriesHandle;

		public DataGenerator()
		{
			_options = new DbContextOptionsBuilder<AppDbContext>()
					.UseNpgsql("Host=localhost;Database=SportsStats;Username=VladislavKulichkov;Password=qw06062013?");
			_context = new(_options.Options);


			_teamRepository = new(_context);
			_tournamentRepository = new(_context);
			_playerRepository = new(_context);
			_matchRepository = new(_context);
			_matchRepository = new(_context);
			_teamStatsRepository = new(_context);

			_matchService = new();

			_teamApplicationService = new(_teamRepository);
			_playerApplicationService = new(_playerRepository, _teamRepository);
			_matchQueriesHandle = new(_matchRepository, _teamRepository, _tournamentRepository, _teamApplicationService, _playerApplicationService);
			_tournamentApplicationService = new(_tournamentRepository, _timeProvider, _teamRepository, _teamStatsRepository, _matchRepository, _matchQueriesHandle);
			_matchLifecycleService = new(_playerRepository, _tournamentRepository, _matchRepository, _teamRepository, _timeProvider, _matchService);
			_matchFinishService = new(_teamStatsRepository, _matchRepository, _timeProvider);
			_matchGoalService = new(_matchRepository, _timeProvider, _matchFinishService);
			_matchRosterService = new(_matchRepository, _playerRepository);
		}


		public async Task<List<int>> GenerateTeamsAsync(ITeamNames teamNames, INamesData namesData)
		{
			var names = teamNames.Names;

			List<int> ids = new List<int>();
			foreach (var name in names)
			{
				var teamId = await _teamApplicationService.CreateAsync(name);
				ids.Add(teamId);

				await GeneratePlayersInTeamAsync(namesData, 4, PositionType.LeftWinger, teamId);
				await GeneratePlayersInTeamAsync(namesData, 4, PositionType.RightWinger, teamId);
				await GeneratePlayersInTeamAsync(namesData, 4, PositionType.Center, teamId);
				await GeneratePlayersInTeamAsync(namesData, 3, PositionType.LeftDefenseman, teamId);
				await GeneratePlayersInTeamAsync(namesData, 3, PositionType.RightDefenseman, teamId);
				await GeneratePlayersInTeamAsync(namesData, 2, PositionType.Goalie, teamId);
			}
			return ids;
		}
		public async Task<List<int>> GeneratePlayersInTeamAsync(INamesData namesData, int count, PositionType position, int teamId)
		{
			var firstNames = namesData.FirstNames;
			var lastNames = namesData.LastNames;

			List<int> players = new();

			for (int i = 1; i <= count; i++)
			{
				int firstNameIndex = _random.Next(firstNames.Count);
				int lastNameIndex = _random.Next(lastNames.Count);

				int playerId = await _playerApplicationService.CreateAsync(firstNames[firstNameIndex], lastNames[lastNameIndex], position);
				await _playerApplicationService.ChangeTeamAsync(playerId, teamId);
				players.Add(playerId);
			}
			return players;
		}
		public async Task<int> GenerateTournamentAsync(string name, ITeamNames teamNames, INamesData namesData, DateTime? startedAt = null)
		{
			int tournamentId = await _tournamentApplicationService.CreateAsync(name);

			//await _tournamentApplicationService.SetRulesAsync(tournamentId, TournamentMapper.ToDTO(TournamentRules.CreateKHLRules()));
			await _tournamentApplicationService.RegistrationAsync(tournamentId);
			var teamIds = await GenerateTeamsAsync(teamNames, namesData);

			await _tournamentApplicationService.SetRegistrationTeamsAsync(tournamentId, teamIds);

			await _tournamentApplicationService.StartAsync(tournamentId, startedAt);

			startedAt = (await _tournamentApplicationService.GetAsync(tournamentId))?.StartedAt;
			DateOnly currentDate = DateOnly.FromDateTime(startedAt.Value);
			var schedule = GenerateSchedule(teamIds);

			foreach (var day in schedule)
			{
				foreach (var match in day)
					await GenerateMatchAsync(match.Item1, match.Item2, tournamentId, DateTime.SpecifyKind(currentDate.ToDateTime(new TimeOnly(19, 30, 0)), DateTimeKind.Utc));
				currentDate = currentDate.AddDays(1);
			}
			return tournamentId;
		}
		public List<List<(int, int)>> GenerateSchedule(List<int> teamIds)
		{
			List<List<(int, int)>> schedule = new();
			List<int> teams = new(teamIds);
			if (teams.Count % 2 == 1)
				teams.Add(0);
			int n = teams.Count;
			for (int i = 0; i < n - 1; i++)
			{
				schedule.Add(new List<(int, int)>());
				schedule.Add(new List<(int, int)>());

				for (int j = 0; j < n / 4; j++)
					if (teams[j] != 0 && teams[n - j - 1] != 0)
						schedule[2 * i].Add((teams[j], teams[n - j - 1]));
				for (int j = n / 4; j < n / 2; j++)
					if (teams[j] != 0 && teams[n - j - 1] != 0)
						schedule[2 * i + 1].Add((teams[j], teams[n - j - 1]));

				var last = teams[n - 1];
				for (int j = n - 1; j > 0; j--)
					teams[j] = teams[j - 1];
				teams[1] = last;

			}
			schedule = schedule.Concat(schedule.Select(d => d.Select(m => (m.Item2, m.Item1)).ToList())).OrderBy(s => _random.Next()).ToList();
			return schedule;
		}
		public async Task<int> GenerateMatchAsync(int homeTeamId, int awayTeamId, int tournamentId, DateTime scheduleAt)
		{
			int matchId = await _matchLifecycleService.CreateAsync(tournamentId, homeTeamId, awayTeamId, scheduleAt);

			await GenerateMatchRosterAsync(matchId, homeTeamId, awayTeamId);
			await _matchLifecycleService.StartAsync(matchId);
			await GenerateGoalsAsync(matchId);
			if (!await _matchQueriesHandle.IsFinished(matchId))
				await _matchFinishService.FinishAsync(matchId);

			return matchId;
		}
		public async Task GenerateMatchRosterAsync(int matchId, int homeTeamId, int awayTeamId)
		{
			var homeTeamPlayers = await _playerApplicationService.GetByteamAsync(homeTeamId);
			var awayTeamPlayers = await _playerApplicationService.GetByteamAsync(awayTeamId);

			var homeTeamPlayerIds = homeTeamPlayers.Select(p => p.Id).ToList();
			var awayTeamPlayerIds = awayTeamPlayers.Select(p => p.Id).ToList();

			await _matchRosterService.SetPlayersToRosterAsync(matchId, homeTeamPlayerIds, homeTeamId);
			await _matchRosterService.SetPlayersToRosterAsync(matchId, awayTeamPlayerIds, awayTeamId);
		}
		public async Task GenerateGoalsAsync(int matchId)
		{
			var match = await _matchQueriesHandle.GetAsync(matchId);

			List<TeamDTO> teams = [match.HomeTeam, match.AwayTeam];
			List<List<PlayerDTO>> rosters = [match.HomeTeamRoster, match.AwayTeamRoster];

			int difference = 0;

			for (int period = 1; period < match.Rules.MatchTimeRules.PeriodsCount + 1; period++)
			{
				int goalsCount = _random.Next(1, 4);
				int lastTime = 0;
				for (int i = 0; i <= goalsCount; i++)
					lastTime = await GenerateGoalAsync(period, lastTime, match.Rules.MatchTimeRules.PeriodDurationSeconds);
			}

			if (difference == 0 && !match.Rules.MatchTimeRules.IsDrawPossible && match.Rules.MatchTimeRules.HasOvertime)
			{
				await GenerateGoalAsync(match.Rules.MatchTimeRules.PeriodsCount + 1, 0, match.Rules.MatchTimeRules.OvertimeRules!.OvertimeDurationSeconds ?? 2400);
			}


			async Task<int> GenerateGoalAsync(int period, int startTime, int endTime)
			{
				int scoringTeamIndex = _random.Next(0, 2);
				difference += scoringTeamIndex == 0 ? 1 : -1;
				int goalScorerIndex = _random.Next(0, rosters[scoringTeamIndex].Count);
				int goalScorerId = rosters[scoringTeamIndex][goalScorerIndex].Id;
				int time = _random.Next(startTime, endTime);

				await _matchGoalService.AddGoalAsync(matchId, teams[scoringTeamIndex].Id, goalScorerId, period, time);
				return time;
			}
		}
		public async Task Start()
		{
			Console.WriteLine("Генерация данных запущена");
			if (!_context.Database.CanConnect())
				Console.WriteLine("Не удалось подключиться");

			int nhlTournamentId = await GenerateTournamentAsync("NHL", new NHLTeamsNames(), new ForeignNamesData());
			int khlTournamentId = await GenerateTournamentAsync("KHL", new KHLTeamNames(), new RussianNamesData());
		}
	}
	public static class Program
	{
		public static async Task Main()
		{

			//var test = new DataGenerator();
			//await test.Start();
			//var playerPhotoHelper = new PhotoHelper();
			//playerPhotoHelper.FillPhoto();

		}


	}
}
