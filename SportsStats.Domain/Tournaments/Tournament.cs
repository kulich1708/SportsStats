using SportsStats.Domain.Common;
using SportsStats.Domain.Tournaments.Rules;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SportsStats.Domain.Tournaments
{
	public class Tournament : BaseEntity, IAggregateRoot
	{
		private List<int> _teamsId = new();
		public string Name { get; private set; }
		public DateTime StartedAt { get; private set; }
		public DateTime FinishedAt { get; private set; }
		public TournamentStatus Status { get; private set; } = TournamentStatus.Draft;

		public TournamentRules TournamentRules { get; private set; }
		public IReadOnlyList<int> TeamsId => _teamsId;

		public Tournament(string name)
		{
			Name = name;
		}
		public void SetRules(TournamentRules tournamentRules)
		{
			if (!IsDraft())
				throw new ArgumentException("Правила можно установить только когда турнир в статусе Draft");
			TournamentRules = tournamentRules;
		}
		public void Start(DateTime startedAt)
		{
			if (_teamsId.Count() < 2)
				throw new ArgumentException("Нельзя начать турнир, если не заявлено как минимум 2 команды");
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
		public bool HasRules() => TournamentRules != null;

		public void RegistrateTeam(int teamId)
		{
			if (!IsRegistration())
				throw new ArgumentException("Можно заявить команду, только когда турнир в статусе Registration");

			_teamsId.Add(teamId);
		}
		public void SetStatus(TournamentStatus status)
		{
			if (status == TournamentStatus.Draft && Status != status)
				throw new ArgumentException("Нельзя установить статус Draft, после любого другого");
			if (status == TournamentStatus.Registration && Status != TournamentStatus.Draft && status != Status && HasRules())
				throw new ArgumentException("Статус Registration  можно установить только если сейчас установлен статус Draft и уже установлены правила турнира");
			if (status == TournamentStatus.InProgress && Status != TournamentStatus.Registration && status != Status)
				throw new ArgumentException("Статус InProgress можно установить только если сейчас установлен статус Registration ");
			if (status == TournamentStatus.Finished && Status != TournamentStatus.InProgress && status != Status)
				throw new ArgumentException("Статус Finished можно установить только если сейчас установлен статус InProgress");

			Status = status;

		}
	}
}
