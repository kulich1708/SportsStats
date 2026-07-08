using SportsStats.Domain.Common;
using SportsStats.Domain.Tournaments.Rules;
using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Tournaments
{
	public class Tournament : BaseEntity, IAggregateRoot
	{
		private readonly HashSet<int> _teamsId = [];
		public string Name { get; private set; }
		public DateTime? StartedAt { get; private set; }
		public DateTime? FinishedAt { get; private set; }
		public TournamentStatus Status { get; private set; } = TournamentStatus.Draft;
		public byte[]? Photo { get; private set; }
		public string? PhotoMime { get; private set; }
		public TournamentRules? TournamentRules { get; private set; }
		public IReadOnlySet<int> TeamsId => _teamsId;

		public Tournament(string name)
		{
			SetName(name);
		}
		public void SetRules(TournamentRules tournamentRules)
		{
			if (!IsDrafted())
				throw new DomainException(TournamentError.RulesCanOnlyBeSetForTournamentInDraftStatus);
			TournamentRules = tournamentRules;
		}
		public void Start(DateTime startedAt)
		{
			if (IsStarted())
				throw new DomainException(TournamentError.TournamentAlreadyStarted);
			if (!IsRegistration())
				throw new DomainException(TournamentError.TournamentCanOnlyBeStartedAfterRegistration);
			if (_teamsId.Count < 2)
				throw new DomainException(TournamentError.TournamentRequiresAtLeastTwoTeams);

			Status = TournamentStatus.InProgress;
			StartedAt = startedAt;
		}
		public void Finish(DateTime finishAt, int unfinishedMatchesCount, DateTime lastMatchFinishedAt)
		{
			if (IsFinished())
				throw new DomainException(TournamentError.TournamentAlreadyFinished);
			if (!IsStarted())
				throw new DomainException(TournamentError.TournamentCanOnlyBeFinishedAfterStart);
			if (StartedAt > finishAt)
				throw new DomainException(TournamentError.TournamentFinishDateCannotBeBeforeStartDate, finishAt, StartedAt.Value);
			if (unfinishedMatchesCount > 0)
				throw new DomainException(TournamentError.TournamentCannotBeFinishedWithUnfinishedMatches, unfinishedMatchesCount.ToString());
			if (finishAt < lastMatchFinishedAt)
				throw new DomainException(TournamentError.TournamentFinishDateCannotBeBeforeLastMatch, finishAt, lastMatchFinishedAt);

			Status = TournamentStatus.Finished;
			FinishedAt = finishAt;
		}
		public void Registration()
		{
			if (IsRegistration())
				throw new DomainException(TournamentError.TournamentRegistrationAlreadyOpen);
			if (!IsDrafted())
				throw new DomainException(TournamentError.RegistrationCanOnlyBeOpenedInDraft);
			if (!HasRules())
				throw new DomainException(TournamentError.RegistrationRequiresRules);

			Status = TournamentStatus.Registration;
		}

		public bool IsDrafted() => Status == TournamentStatus.Draft;
		public bool IsRegistration() => Status == TournamentStatus.Registration;
		public bool IsStarted() => Status == TournamentStatus.InProgress;
		public bool IsFinished() => Status == TournamentStatus.Finished;
		public bool HasRules() => TournamentRules != null;

		public void SetRegistrationTeams(List<int> teamIds)
		{
			_teamsId.Clear();

			foreach (int teamId in teamIds)
				RegistrateTeam(teamId);
		}
		public void RegistrateTeam(int teamId)
		{
			if (!IsRegistration())
				throw new DomainException(TournamentError.TeamsCanOnlyBeRegisteredInRegistrationStatus);
			if (_teamsId.Contains(teamId))
				throw new DomainException(TournamentError.TeamAlreadyRegisteredForTournament);
			_teamsId.Add(teamId);
		}

		public void SetPhoto(byte[] photo, string photoMime)
		{
			Photo = photo;
			PhotoMime = photoMime;
		}
		public void SetName(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new DomainException(TournamentError.TournamentNameCannotBeEmpty);

			Name = name;
		}
	}
}
