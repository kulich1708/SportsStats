//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Options;
//using SportsStats.Application.Matches;
//using SportsStats.Application.Players;
//using SportsStats.Application.Teams;
//using SportsStats.Application.Tournaments;
//using SportsStats.Domain.Common;
//using SportsStats.Domain.Matches;
//using SportsStats.Domain.Matches.Goals;
//using SportsStats.Domain.Players;
//using SportsStats.Domain.Shared;
//using SportsStats.Domain.Teams;
//using SportsStats.Domain.Tournaments;
//using SportsStats.Infrastructure.Persistence.DbContexts;
//using SportsStats.Infrastructure.Persistence.Repositories;
//using System.Numerics;
//using System.Text;

//namespace SportsStats.ConsoleApp
//{
//	public static class PrintExtension
//	{
//		public static string Print(this Player player)
//		{
//			string result = $"{player.Name} {player.Surname}, id: {player.Id}\nКоманда: {player.TeamId}\nПозиция: {player.Position}";
//			return result;
//		}
//		public static string Print(this Team team)
//		{
//			string result = $"{team.Name}, id: {team.Id}";
//			return result;
//		}
//		public static string Print(this Tournament tournament)
//		{
//			string result = $"{tournament.Name}, id: {tournament.Id}";
//			return result;
//		}
//		public async static Task<string> Print(this Match match, ITeamRepository teamRepository, IPlayerRepository playerRepository)
//		{
//			var stringBuilder = new StringBuilder();
//			stringBuilder.AppendLine($"Match ID: {match.Id}");
//			stringBuilder.AppendLine($"{(await teamRepository.FindById(match.HomeTeamId)).Name}    {match.HomeTeamScore}:{match.AwayTeamScore}    {(await teamRepository.FindById(match.AwayTeamId)).Name}");
//			stringBuilder.AppendLine($"tournament ID: {match.TournamentId}");
//			stringBuilder.AppendLine($"Status: {match.Status}");
//			stringBuilder.AppendLine($"Home team roster: {string.Join(" ", match.HomeTeamRoster)}");
//			stringBuilder.AppendLine($"Away team roster: {string.Join(" ", match.AwayTeamRoster)}");
//			stringBuilder.AppendLine($"Home team win type: {match.HomeTeamWinType}");
//			stringBuilder.AppendLine($"Away team win type: {match.AwayTeamWinType}");
//			stringBuilder.AppendLine($"Is overtime: {match.IsOvertime}");

//			foreach (var goal in match.Goals)
//				stringBuilder.AppendLine(await goal.Print(teamRepository, playerRepository));

//			return stringBuilder.ToString();
//		}
//		public async static Task<string> Print(this GoalEvent goal, ITeamRepository teamRepository, IPlayerRepository playerRepository)
//		{
//			var stringBuilder = new StringBuilder();
//			stringBuilder.AppendLine($"Гол команды {(await teamRepository.FindById(goal.ScoringTeamId)).Name}");
//			stringBuilder.Append($"{(await playerRepository.FindById(goal.GoalScorerId)).Surname}");
//			if (goal.FirstAssistId.HasValue)
//			{
//				stringBuilder.Append($" ({(await playerRepository.FindById(goal.FirstAssistId.Value)).Surname}");
//				if (goal.SecondAssistId.HasValue)
//					stringBuilder.Append($", {(await playerRepository.FindById(goal.SecondAssistId.Value)).Surname}");
//				stringBuilder.Append(")");
//			}
//			if (goal.StrengthType.HasValue)
//				stringBuilder.Append($" {goal.StrengthType.Value.GetDescription()}");
//			if (goal.NetType.HasValue)
//				stringBuilder.Append(goal.NetType.Value.GetDescription());
//			stringBuilder.AppendLine();
//			stringBuilder.AppendLine($"{goal.Period} период, {goal.Time / 60} минута {goal.Time % 60} секунда");

//			return stringBuilder.ToString();
//		}
//	}
//	public class Test
//	{
//		public TimeProvider timeProvider = new();
//		public Random Random = new();
//		public DbContextOptionsBuilder<AppDbContext> Options { get; private set; }
//		public AppDbContext Context { get; private set; }
//		public PlayerRepository PlayerRepository { get; private set; }
//		public TeamRepository TeamRepository { get; private set; }
//		public TournamentRepository TournamentRepository { get; private set; }
//		public MatchRepository MatchRepository { get; private set; }

//		public PlayerApplicationService PlayerApplicationService { get; private set; }
//		public TeamApplicationService TeamApplicationService { get; private set; }
//		public TournamentApplicationService TournamentApplicationService { get; private set; }
//		public MatchApplicationService MatchApplicationService { get; private set; }

//		public Test()
//		{
//			Options = new DbContextOptionsBuilder<AppDbContext>()
//					.UseNpgsql("Host=localhost;Database=SportsStats;Username=VladislavKulichkov;Password=qw06062013?");
//			Context = new(Options.Options);


//			TeamRepository = new(Context);
//			TournamentRepository = new(Context);
//			PlayerRepository = new(Context);
//			MatchRepository = new(Context);

//			TeamApplicationService = new(TeamRepository);
//			TournamentApplicationService = new(TournamentRepository, timeProvider, TeamRepository);
//			MatchApplicationService = new(PlayerRepository, TournamentRepository, MatchRepository, timeProvider);
//			PlayerApplicationService = new(PlayerRepository, TeamRepository);
//		}

//		public async Task<List<int>> CreateTeams()
//		{
//			List<int> ids = new List<int>();
//			var team1 = await TeamApplicationService.Create("АВАНГАРД");
//			var team2 = await TeamApplicationService.Create("Ска");
//			ids.Add(team1.Id);
//			ids.Add(team2.Id);
//			Console.WriteLine("teams created");
//			return ids;
//		}

//		public async Task<List<Player>> CreatePlayers(int count, PositionType position, List<int> teamsId)
//		{
//			// Список из 40 имён
//			List<string> firstNames = new List<string>
//		{
//			"Александр", "Дмитрий", "Максим", "Сергей", "Андрей", "Алексей", "Иван", "Егор",
//			"Никита", "Михаил", "Владимир", "Павел", "Роман", "Олег", "Кирилл", "Денис",
//			"Артём", "Илья", "Глеб", "Матвей", "Тимофей", "Юрий", "Василий", "Григорий",
//			"Евгений", "Николай", "Пётр", "Степан", "Фёдор", "Ярослав", "Антон", "Вячеслав",
//			"Константин", "Леонид", "Руслан", "Станислав", "Виктор", "Валентин", "Семён", "Даниил"
//		};

//			// Список из 40 фамилий
//			List<string> lastNames = new List<string>
//		{
//			"Иванов", "Петров", "Сидоров", "Кузнецов", "Смирнов", "Васильев", "Михайлов", "Новиков",
//			"Фёдоров", "Морозов", "Волков", "Алексеев", "Лебедев", "Семёнов", "Егоров", "Павлов",
//			"Козлов", "Степанов", "Николаев", "Орлов", "Андреев", "Макаров", "Никитин", "Захаров",
//			"Соловьёв", "Борисов", "Яковлев", "Григорьев", "Романов", "Воробьёв", "Сергеев", "Кузьмин",
//			"Фролов", "Александров", "Дмитриев", "Королёв", "Гусев", "Киселёв", "Ильин", "Максимов"
//		};

//			List<Player> players = new();

//			for (int i = 1; i <= count; i++)
//			{
//				int firstNameIndex = Random.Next(firstNames.Count);
//				int lastNameIndex = Random.Next(lastNames.Count);

//				Player player = await PlayerApplicationService.Create(firstNames[firstNameIndex], lastNames[lastNameIndex], position);
//				players.Add(player);
//			}
//			return players;
//		}
//		public async Task<int> CreateTournament(List<int> teamsId)
//		{
//			Tournament tournament = await TournamentApplicationService.Create("Кубок открытия");

//			await Context.SaveChangesAsync();
//			await TournamentApplicationService.SetRules(tournament.Id);
//			await TournamentApplicationService.SetStatus(tournament.Id, TournamentStatus.Registration);

//			foreach (int teamId in teamsId)
//				await TournamentApplicationService.RegistrateTeam(tournament.Id, teamId);

//			return tournament.Id;
//		}
//		public async Task Start()
//		{
//			Console.WriteLine("Тест запущен, проверка соединения");
//			if (!Context.Database.CanConnect())
//				Console.WriteLine("Не удалось подключиться");

//			List<int> teamsId = await CreateTeams();
//			int tournamentId = await CreateTournament(teamsId);

//			//int tournamentId = Context.Tournaments.Where(t => t.Status == TournamentStatus.Registration).FirstOrDefault()?.Id ?? 0;
//			//List<int> teamsId = Context.Tournaments.Where(t => t.Status == TournamentStatus.Registration).FirstOrDefault()?.TeamsId.ToList() ?? [];

//			IEnumerable<Player> newPlayers = [];
//			newPlayers = newPlayers.Concat(await CreatePlayers(8, PositionType.LeftWinger, teamsId));
//			newPlayers = newPlayers.Concat(await CreatePlayers(8, PositionType.RightWinger, teamsId));
//			newPlayers = newPlayers.Concat(await CreatePlayers(8, PositionType.Center, teamsId));
//			newPlayers = newPlayers.Concat(await CreatePlayers(6, PositionType.LeftDefenseman, teamsId));
//			newPlayers = newPlayers.Concat(await CreatePlayers(6, PositionType.RightDefenseman, teamsId));
//			newPlayers = newPlayers.Concat(await CreatePlayers(4, PositionType.Goalie, teamsId));
//			List<Player> newPlayersList = newPlayers.ToList();

//			Console.WriteLine(string.Join(" ", newPlayersList.Select(p => p.Id)));

//			for (int i = 0; i < newPlayersList.Count; i++)
//				await PlayerApplicationService.ChangeTeam(newPlayersList[i].Id, teamsId[(i) % teamsId.Count]);

//			Console.WriteLine("Список всех игроков:");
//			await Context.Players.ForEachAsync(player => Console.WriteLine(player.Print()));
//			Console.WriteLine();
//			Console.WriteLine();


//			Match match = await MatchApplicationService.CreateMatch(tournamentId, teamsId[0], teamsId[1]);

//			int matchId = match.Id;
//			foreach (var player in newPlayersList)
//				await MatchApplicationService.AddPlayerToRoster(matchId, player.Id, player.TeamId);

//			await MatchApplicationService.StartMatch(matchId);

//			List<Player> HomeTeamPlayers = await Context.Players.Where(player => player.TeamId == match.HomeTeamId).ToListAsync();
//			List<Player> AwayTeamPlayers = await Context.Players.Where(player => player.TeamId == match.AwayTeamId).ToListAsync();

//			Console.WriteLine(await match.Print(TeamRepository, PlayerRepository));
//			Console.WriteLine();
//			Console.WriteLine();

//			GoalEvent firstGoal = await MatchApplicationService.AddGoal(matchId, teamsId[0], HomeTeamPlayers[0].Id, 1, 20);

//			Console.WriteLine(await match.Print(TeamRepository, PlayerRepository));
//			Console.WriteLine();
//			Console.WriteLine();
//			await MatchApplicationService.AddGoal(matchId, teamsId[0], HomeTeamPlayers[0].Id, 1, 1199);
//			await MatchApplicationService.AddGoal(matchId, teamsId[1], AwayTeamPlayers[0].Id, 2, 20);
//			await MatchApplicationService.AddGoal(matchId, teamsId[1], AwayTeamPlayers[1].Id, 3, 1199);
//			await MatchApplicationService.AddGoal(matchId, teamsId[0], HomeTeamPlayers[0].Id, 4, 299);

//			Console.WriteLine(await match.Print(TeamRepository, PlayerRepository));
//			Console.WriteLine();
//			Console.WriteLine();
//			await MatchApplicationService.FillGoalDetails(matchId, firstGoal.Id, HomeTeamPlayers[1].Id, HomeTeamPlayers[2].Id, GoalStrengthType.EvenStrength);

//			Match matchInDb = await MatchRepository.FindById(matchId);
//			Console.WriteLine(await match.Print(TeamRepository, PlayerRepository));
//			Console.WriteLine(await matchInDb.Print(TeamRepository, PlayerRepository));
//		}
//	}
//	public static class Program
//	{
//		public static async Task Main()
//		{
//			var test = new Test();
//			await test.Start();
//		}

//	}
//	public class TimeProvider : ITimeProvider
//	{
//		public DateTime GetCurrentTime()
//		{
//			return DateTime.UtcNow;
//		}
//	}
//}