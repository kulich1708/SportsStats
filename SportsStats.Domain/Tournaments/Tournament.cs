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
		private readonly List<int> _teamsId = [];
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
			if (!IsDrafted())
				throw new ArgumentException("Правила можно установить только когда турнир в статусе Draft");
			TournamentRules = tournamentRules;
		}
		public void Start(DateTime startedAt)
		{
			if (IsStarted())
				throw new ArgumentException("Турнир уже начат");
			if (!IsRegistration())
				throw new ArgumentException("Турнир можно начать только после регистрации команд");
			if (_teamsId.Count < 2)
				throw new ArgumentException("Нельзя начать турнир, если не заявлено как минимум 2 команды");

			Status = TournamentStatus.InProgress;
			StartedAt = startedAt;
		}
		public void Finish(DateTime finishAt)
		{
			if (IsFinished())
				throw new AggregateException("Туринр уже завершён");
			if (!IsStarted())
				throw new ArgumentException("Турнир можно завершить только после того, как он начался");

			Status = TournamentStatus.Finished;
			FinishedAt = finishAt;
		}
		public void Registration()
		{
			if (IsRegistration())
				throw new ArgumentException("Турнир уже открыт для регистрации команд");
			if (!IsDrafted())
				throw new ArgumentException("Открыть регистрацию команд можно только если турнир находится в статусе Draft");
			if (!HasRules())
				throw new ArgumentException("Открыть регистрацию команд можно только если турниру уже установлены правила");

			Status = TournamentStatus.Registration;
		}

		public bool IsDrafted() => Status == TournamentStatus.Draft;
		public bool IsRegistration() => Status == TournamentStatus.Registration;
		public bool IsStarted() => Status == TournamentStatus.InProgress;
		public bool IsFinished() => Status == TournamentStatus.Finished;
		public bool HasRules() => TournamentRules != null && TournamentRules.HasRules();

		public void RegistrateTeam(int teamId)
		{
			if (!IsRegistration())
				throw new ArgumentException("Можно заявить команду, только когда турнир в статусе Registration");
			if (_teamsId.Contains(teamId))
				throw new ArgumentException("Команда уже заявлена на этот турнир");
			_teamsId.Add(teamId);
		}
	}
}
