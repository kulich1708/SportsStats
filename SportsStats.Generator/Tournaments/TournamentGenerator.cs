using ConsoleApp.Matches;
using SportsStats.Application.Tournaments;
using SportsStats.Application.Tournaments.Mappers.Rules;
using SportsStats.Domain.Shared;
using SportsStats.Domain.Tournaments.Rules;
using SportsStats.Generator.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ConsoleApp.Tournaments
{
	public class TournamentGenerator(
		TournamentApplicationService tournamentApplicationService,
		MatchGenerator matchGenerator,
		ITimeProvider timeProvider)
	{
		private readonly TournamentApplicationService _tournamentApplicationService = tournamentApplicationService;
		private readonly MatchGenerator _matchGenerator = matchGenerator;
		private readonly ITimeProvider _timeProvider = timeProvider;
		private readonly Random _random = new();
		public async Task<int> GenerateTournamentAsync
			(string name, List<int> teamIds, DateTime? startedAt = null)
		{
			var sw = Stopwatch.StartNew();

			int tournamentId = await _tournamentApplicationService.CreateAsync(name);

			await _tournamentApplicationService.SetRulesAsync(tournamentId, MatchRulesMapper.ToDTO(TournamentRules.CreateKHLRules()));
			await _tournamentApplicationService.RegistrationAsync(tournamentId);

			await _tournamentApplicationService.SetRegistrationTeamsAsync(tournamentId, teamIds);

			var schedule = GenerateSchedule(teamIds);
			int scheduleDays = schedule.Count;

			startedAt = startedAt ?? _timeProvider.GetCurrentTime().AddDays(-1 * (scheduleDays / 2));
			await _tournamentApplicationService.StartAsync(tournamentId, startedAt);

			startedAt = (await _tournamentApplicationService.GetAsync(tournamentId))?.StartedAt;
			DateOnly currentDate = DateOnly.FromDateTime(startedAt.Value).AddDays(1);
			DateTime time = DateTime.SpecifyKind(currentDate.ToDateTime(new TimeOnly(19, 30, 0)), DateTimeKind.Utc);

			int matchCount = schedule.Select(d => d.Count).Sum();
			Console.WriteLine($"Начало генерации матчей в турнире. Всего матчей - {matchCount}");
			int generatedMatchCount = 0;
			for (int i = 0; i < scheduleDays; i++)
			{
				var day = schedule[i];
				foreach (var match in day)
				{
					if (generatedMatchCount % 50 == 0)
						Console.WriteLine($"Сгенерировано {generatedMatchCount}, осталось {matchCount - generatedMatchCount}");
					generatedMatchCount++;

					if (i >= scheduleDays / 2)
						await _matchGenerator.CreateMatchAsync(match.Item1, match.Item2, tournamentId, time);
					else
						await _matchGenerator.GenerateMatchAsync(
							match.Item1, match.Item2, tournamentId, time);
				}
				currentDate = currentDate.AddDays(1);
				time = DateTime.SpecifyKind(currentDate.ToDateTime(new TimeOnly(19, 30, 0)), DateTimeKind.Utc);
			}

			sw.Log("Все матчи турнира сгенерированы");
			Console.WriteLine();
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
	}
}
