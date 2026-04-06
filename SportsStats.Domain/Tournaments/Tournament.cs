using SportsStats.Domain.Common;
using SportsStats.Domain.Tournaments.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments
{
	public class Tournament : BaseEntity, IAggregateRoot
	{
		private readonly TournamentRules _tournamentRules;
		private List<int> _teamsId;
		public string Name { get; private set; }
		public DateTime StartedAt { get; private set; }
		public DateTime FinishedAt { get; private set; }
		public TournamentStatus Status { get; private set; } = TournamentStatus.Waiting;
		public TournamentRules TournamentRules => _tournamentRules;
		public IReadOnlyList<int> TeamsId => _teamsId;

		public Tournament(string name, TournamentRules tournamentRules)
		{
			Name = name;
			_tournamentRules = tournamentRules;
		}
		public void Start(DateTime startedAt)
		{
			if (_teamsId.Count() < 2)
				throw new AggregateException("Нельзя начать турнир, если не заявлено как минимум 2 команды");
			Status = TournamentStatus.InProgress;
			StartedAt = startedAt;
		}
		public void Finish(DateTime finishAt)
		{
			Status = TournamentStatus.Finished;
			FinishedAt = finishAt;
		}

		public bool IsWaiting() => Status == TournamentStatus.Waiting;
		public bool IsInProgress() => Status == TournamentStatus.InProgress;
		public bool IsFinished() => Status == TournamentStatus.Finished;

		public void RegistrateTeam(int teamId)
		{
			if (!IsWaiting())
				throw new ArgumentException("Можно заявить команду, только когда турнир ещё не начат");

			_teamsId.Add(teamId);
		}
	}
}
