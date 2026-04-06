using SportsStats.Domain.Common;
using SportsStats.Domain.Tournaments.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments
{
	public class Tournament : BaseEntity, IAggregateRoot
	{
		private TournamentRules _tournamentRules;
		private List<int> _teamsId = new();
		public string Name { get; private set; }
		public DateTime StartedAt { get; private set; }
		public DateTime FinishedAt { get; private set; }
		public TournamentStatus Status { get; private set; } = TournamentStatus.Draft;
		public TournamentRules TournamentRules => _tournamentRules;
		public IReadOnlyList<int> TeamsId => _teamsId;

		public Tournament(string name)
		{
			Name = name;
		}
		public void SetRules(TournamentRules tournamentRules)
		{
			if (!IsDraft())
				throw new ArgumentException("Правила можно установить только когда турнир в статусе Draft");
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

		public bool IsDraft() => Status == TournamentStatus.Draft;
		public bool IsRegistration() => Status == TournamentStatus.Registration;
		public bool IsInProgress() => Status == TournamentStatus.InProgress;
		public bool IsFinished() => Status == TournamentStatus.Finished;
		public bool HasRules() => _tournamentRules != null;

		public void RegistrateTeam(int teamId)
		{
			if (!IsRegistration())
				throw new ArgumentException("Можно заявить команду, только когда турнир в статусе Registration");

			_teamsId.Add(teamId);
		}
		public void SetStatus(TournamentStatus status)
		{
			if (status == TournamentStatus.Draft && Status != status)
				throw new AggregateException("Нельзя установить статус Draft, после любого другого");
			if (status == TournamentStatus.Registration && Status != TournamentStatus.Draft && status != Status && HasRules())
				throw new AggregateException("Статус Registration  можно установить только если сейчас установлен статус Draft и уже установлены правила турнира");
			if (status == TournamentStatus.InProgress && Status != TournamentStatus.Registration && status != Status)
				throw new AggregateException("Статус InProgress можно установить только если сейчас установлен статус Registration ");
			if (status == TournamentStatus.Finished && Status != TournamentStatus.InProgress && status != Status)
				throw new AggregateException("Статус Finished можно установить только если сейчас установлен статус InProgress");

			Status = status;

		}
	}
}
