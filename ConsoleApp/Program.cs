using SportsStats.Application.Matches;
using SportsStats.Application.Players;
using SportsStats.Application.Teams;
using SportsStats.Application.Tournaments;
using SportsStats.Domain.Common;
using SportsStats.Domain.Players;
using SportsStats.Domain.Shared;
using SportsStats.Domain.Teams;
using SportsStats.Domain.Tournaments;
using SportsStats.Domain.Matches;
using SportsStats.Domain.Matches.Goals;
using System.Numerics;
using System.Text;

namespace SportsStats.ConsoleApp
{
	public static class Program
	{
		public static Random random = new Random();
		public static InMemoryPlayerRepository inMemoryPlayerRepository = new InMemoryPlayerRepository();
		public static InMemoryTeamRepository inMemoryTeamRepository = new InMemoryTeamRepository();
		public static InMemoryTournamentRepository inMemoryTournamentRepository = new InMemoryTournamentRepository();
		public static InMemoryMatchRepository inMemoryMatchRepository = new InMemoryMatchRepository();
		public static InMemoryGoalRepository InMemoryGoalRepository = new InMemoryGoalRepository();
		public static TimeProvider timeProvider = new TimeProvider();

		public static PlayerApplicationService playerApplicationService = new PlayerApplicationService(inMemoryPlayerRepository,
			inMemoryTeamRepository);
		public static TeamApplicationService teamApplicationService = new TeamApplicationService(inMemoryTeamRepository);
		public static TournamentApplicationService tournamentApplicationService = new(inMemoryTournamentRepository, timeProvider,
			inMemoryTeamRepository);
		public static MatchApplicationService matchApplicationService = new(inMemoryPlayerRepository, inMemoryTournamentRepository,
			inMemoryMatchRepository, InMemoryGoalRepository, timeProvider);
		public static string Print(this Player player)
		{
			string result = $"{player.Name} {player.Surname}, id: {player.Id}\nКоманда: {player.TeamId}\nПозиция: {player.Position}";
			return result;
		}
		public static string Print(this Team team)
		{
			string result = $"{team.Name}, id: {team.Id}";
			return result;
		}
		public static string Print(this Tournament tournament)
		{
			string result = $"{tournament.Name}, id: {tournament.Id}";
			return result;
		}
		public static string Print(this Match match)
		{
			var stringBuilder = new StringBuilder();
			stringBuilder.AppendLine($"{inMemoryTeamRepository.FindById(match.HomeTeamId).Name}    {match.HomeTeamScore}:{match.AwayTeamScore}    {inMemoryTeamRepository.FindById(match.AwayTeamId).Name}");
			stringBuilder.AppendLine($"tournament ID: {match.TournamentId}");
			stringBuilder.AppendLine($"Status: {match.Status}");
			stringBuilder.AppendLine($"Home team roster: {string.Join(" ", match.HomeTeamRoster)}");
			stringBuilder.AppendLine($"Away team roster: {string.Join(" ", match.AwayTeamRoster)}");
			stringBuilder.AppendLine($"Home team win type: {match.HomeTeamWinType}");
			stringBuilder.AppendLine($"Away team win type: {match.AwayTeamWinType}");
			stringBuilder.AppendLine($"Is overtime: {match.IsOvertime}");

			foreach (var goal in match.Goals)
				stringBuilder.AppendLine(goal.Print());

			return stringBuilder.ToString();
		}
		public static string Print(this GoalEvent goal)
		{
			var stringBuilder = new StringBuilder();
			stringBuilder.AppendLine($"Гол команды {inMemoryTeamRepository.FindById(goal.ScoringTeamId).Name}");
			stringBuilder.Append($"{inMemoryPlayerRepository.FindById(goal.GoalScorerId).Surname}");
			if (goal.FirstAssistId.HasValue)
			{
				stringBuilder.Append($" ({inMemoryPlayerRepository.FindById(goal.FirstAssistId.Value).Surname}");
				if (goal.SecondAssistId.HasValue)
					stringBuilder.Append($", {inMemoryPlayerRepository.FindById(goal.SecondAssistId.Value).Surname}");
				stringBuilder.Append(")");
			}
			if (goal.StrengthType.HasValue)
				stringBuilder.Append($" {goal.StrengthType.Value.GetDescription()}");
			if (goal.NetType.HasValue)
				stringBuilder.Append(goal.NetType.Value.GetDescription());
			stringBuilder.AppendLine();
			stringBuilder.AppendLine($"{goal.Period} период, {goal.Time / 60} минута {goal.Time % 60} секунда");

			return stringBuilder.ToString();
		}

		public static List<int> CreateTeams()
		{
			List<int> ids = new List<int>();
			ids.Add(teamApplicationService.Create("АВАНГАРД").Id);
			ids.Add(teamApplicationService.Create("Ска").Id);
			return ids;
		}
		public static List<int> CreatePlayers(int count, PositionType position, List<int> teamsId)
		{
			// Список из 40 имён
			List<string> firstNames = new List<string>
		{
			"Александр", "Дмитрий", "Максим", "Сергей", "Андрей", "Алексей", "Иван", "Егор",
			"Никита", "Михаил", "Владимир", "Павел", "Роман", "Олег", "Кирилл", "Денис",
			"Артём", "Илья", "Глеб", "Матвей", "Тимофей", "Юрий", "Василий", "Григорий",
			"Евгений", "Николай", "Пётр", "Степан", "Фёдор", "Ярослав", "Антон", "Вячеслав",
			"Константин", "Леонид", "Руслан", "Станислав", "Виктор", "Валентин", "Семён", "Даниил"
		};

			// Список из 40 фамилий
			List<string> lastNames = new List<string>
		{
			"Иванов", "Петров", "Сидоров", "Кузнецов", "Смирнов", "Васильев", "Михайлов", "Новиков",
			"Фёдоров", "Морозов", "Волков", "Алексеев", "Лебедев", "Семёнов", "Егоров", "Павлов",
			"Козлов", "Степанов", "Николаев", "Орлов", "Андреев", "Макаров", "Никитин", "Захаров",
			"Соловьёв", "Борисов", "Яковлев", "Григорьев", "Романов", "Воробьёв", "Сергеев", "Кузьмин",
			"Фролов", "Александров", "Дмитриев", "Королёв", "Гусев", "Киселёв", "Ильин", "Максимов"
		};

			List<int> playersId = new();

			for (int i = 1; i <= count; i++)
			{
				int firstNameIndex = random.Next(firstNames.Count);
				int lastNameIndex = random.Next(lastNames.Count);

				Player player = playerApplicationService.Create(firstNames[firstNameIndex], lastNames[lastNameIndex], position);
				playerApplicationService.ChangeTeam(player.Id, teamsId[(i - 1) % teamsId.Count]);
				playersId.Add(player.Id);
			}
			return playersId;
		}
		public static int CreateTournament(List<int> teamsId)
		{
			Tournament tournament = tournamentApplicationService.Create("Кубок открытия");
			tournamentApplicationService.SetRules(tournament.Id);
			tournamentApplicationService.SetStatus(tournament.Id, TournamentStatus.Registration);

			foreach (int teamId in teamsId)
				tournamentApplicationService.RegistrateTeam(tournament.Id, teamId);

			return tournament.Id;
		}
		public static void Main()
		{
			List<int> teamsId = CreateTeams();
			inMemoryTeamRepository.Print();


			CreatePlayers(8, PositionType.LeftWinger, teamsId);
			CreatePlayers(8, PositionType.RightWinger, teamsId);
			CreatePlayers(8, PositionType.Center, teamsId);
			CreatePlayers(6, PositionType.LeftDefenseman, teamsId);
			CreatePlayers(6, PositionType.RightDefenseman, teamsId);
			CreatePlayers(4, PositionType.Goalie, teamsId);
			List<Player> players = inMemoryPlayerRepository.Players.Values.ToList();
			Console.WriteLine("Список всех игроков:\n");
			inMemoryPlayerRepository.Print();

			int tournamentId = CreateTournament(teamsId);
			inMemoryTournamentRepository.Print();
			int teamIdWithoutTournament = teamApplicationService.Create("ЦСКА").Id;


			int matchId = matchApplicationService.CreateMatch(tournamentId, teamsId[0], teamsId[1]).Id;
			players.ForEach(player => matchApplicationService.AddPlayerToRoster(matchId, player.Id, player.TeamId));
			matchApplicationService.StartMatch(matchId);
			matchApplicationService.AddGoal(matchId, teamsId[0], 1, 1, 20);
			matchApplicationService.AddGoal(matchId, teamsId[0], 1, 1, 1199);
			matchApplicationService.AddGoal(matchId, teamsId[1], 2, 2, 20);
			matchApplicationService.AddGoal(matchId, teamsId[1], 4, 3, 1199);
			matchApplicationService.AddGoal(matchId, teamsId[0], 1, 4, 299);
			matchApplicationService.FillGoalDetails(matchId, 1, 3, 5, GoalStrengthType.EvenStrength);


			Match match = inMemoryMatchRepository.FindById(matchId);
			Console.WriteLine(match.Print());
		}
	}
	public class InMemoryRepository
	{
		public void SetId(BaseEntity entity, ref int nextId)
		{
			// Симулируем генерацию ID как в реальной БД
			if (entity.Id == 0) // или default
			{
				entity.SetId(nextId++); // Нужен метод для установки ID
			}
		}
	}
	public class InMemoryPlayerRepository : InMemoryRepository, IPlayerRepository
	{
		private Dictionary<int, Player> _players = new();
		private int _nextId = 1;
		public IReadOnlyDictionary<int, Player> Players => _players;

		public Player FindById(int playerId)
		{
			return _players.TryGetValue(playerId, out Player player) ? player : null;
		}
		public Player Save(Player player)
		{
			SetId(player, ref _nextId);
			return _players[player.Id] = player;
		}
		public void Print()
		{
			foreach (var item in _players)
			{
				Console.WriteLine($"{item.Key}: {item.Value.Print()}");
				Console.WriteLine();
			}
		}
	}
	public class InMemoryTeamRepository : InMemoryRepository, ITeamRepository
	{
		private Dictionary<int, Team> _teams = new();
		private int _nextId = 1;
		public IReadOnlyDictionary<int, Team> Teams => _teams;
		public Team FindById(int teamId)
		{
			return _teams.TryGetValue(teamId, out Team team) ? team : null;
		}

		public Team Save(Team team)
		{
			SetId(team, ref _nextId);
			return _teams[team.Id] = team;
		}
		public void Print()
		{
			foreach (var item in _teams)
			{
				Console.WriteLine($"{item.Key}: {item.Value.Print()}");
				Console.WriteLine();
			}
		}
	}
	public class InMemoryTournamentRepository : InMemoryRepository, ITournamentRepository
	{

		private Dictionary<int, Tournament> _tournaments = new();
		private int _nextId = 1;
		public IReadOnlyDictionary<int, Tournament> Tournaments => _tournaments;
		public Tournament FindById(int teamId)
		{
			return _tournaments.TryGetValue(teamId, out Tournament team) ? team : null;
		}

		public Tournament Save(Tournament team)
		{
			SetId(team, ref _nextId);
			return _tournaments[team.Id] = team;
		}
		public void Print()
		{
			foreach (var item in _tournaments)
			{
				Console.WriteLine($"{item.Key}: {item.Value.Print()}");
				Console.WriteLine();
			}
		}
	}
	public class InMemoryMatchRepository : InMemoryRepository, IMatchRepository
	{

		private Dictionary<int, Match> _matches = new();
		private int _nextId = 1;
		public IReadOnlyDictionary<int, Match> Matches => _matches;
		public Match FindById(int matchId)
		{
			return _matches.TryGetValue(matchId, out Match match) ? match : null;
		}

		public Match Save(Match match)
		{
			SetId(match, ref _nextId);
			return _matches[match.Id] = match;
		}
		public void Print()
		{
			foreach (var item in _matches)
			{
				Console.WriteLine($"Match ID: {item.Key}: \n{item.Value.Print()}");
				Console.WriteLine();
			}
		}
	}
	public class InMemoryGoalRepository : InMemoryRepository, IGoalRepository
	{

		private Dictionary<int, GoalEvent> _goals = new();
		private int _nextId = 1;
		public IReadOnlyDictionary<int, GoalEvent> Goals => _goals;
		public GoalEvent FindById(int goalId)
		{
			return _goals.TryGetValue(goalId, out GoalEvent goal) ? goal : null;
		}

		public GoalEvent Save(GoalEvent goal)
		{
			SetId(goal, ref _nextId);
			return _goals[goal.Id] = goal;
		}
		public void Print()
		{
			foreach (var item in _goals)
			{
				Console.WriteLine($"Match ID: {item.Key}: \n{item.Value.Print()}");
				Console.WriteLine();
			}
		}
	}
	public class TimeProvider : ITimeProvider
	{
		public DateTime GetCurrentTime()
		{
			return DateTime.Now;
		}
	}
}