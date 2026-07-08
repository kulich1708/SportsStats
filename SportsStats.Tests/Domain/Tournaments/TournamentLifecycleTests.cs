using SportsStats.Domain.Matches;
using SportsStats.Domain.Shared;
using SportsStats.Domain.Tournaments;
using SportsStats.Domain.Tournaments.Rules;
using System;
using Xunit;

namespace SportsStats.Tests.Domain.Tournaments
{
	public class TournamentLifecycleTests
	{
		[Fact]
		public void Registration_WithRulesInDraft_ChangesStatusToRegistration()
		{
			var tournament = new Tournament("KHL Test");
			var rules = TournamentRules.CreateKHLRules();
			tournament.SetRules(rules);

			tournament.Registration();

			Assert.True(tournament.IsRegistration());
		}

		[Fact]
		public void Registration_WhithoutRulesInDraft_ThrowsArgumentException()
		{
			var tournament = new Tournament("KHL test");

			var ex = Assert.Throws<DomainException>(tournament.Registration);

			Assert.Equal(TournamentError.RegistrationRequiresRules.Code, ex.Code);
		}
		[Fact]
		public void Start_InDraft_ThrowsArgumentException()
		{
			var tournament = new Tournament("KHL test");
			DateTime startAt = new(2026, 4, 29);

			var ex = Assert.Throws<DomainException>(() => tournament.Start(startAt));

			Assert.Equal(TournamentError.TournamentCanOnlyBeStartedAfterRegistration.Code, ex.Code);
		}
		[Fact]
		public void Start_WithLessThanTwoTeams_ThrowsArgumentExceptoin()
		{
			var tournament = new Tournament("KHL test");
			var rules = TournamentRules.CreateKHLRules();
			tournament.SetRules(rules);
			tournament.Registration();
			DateTime startAt = new(2026, 4, 29);

			var ex = Assert.Throws<DomainException>(() => tournament.Start(startAt));

			Assert.Equal(TournamentError.TournamentRequiresAtLeastTwoTeams.Code, ex.Code);
		}
		[Fact]
		public void Start_WithTwoTeams_ChangesStatusToInProgress()
		{
			var tournament = new Tournament("KHL test");
			var rules = TournamentRules.CreateKHLRules();
			tournament.SetRules(rules);
			tournament.Registration();
			tournament.RegistrateTeam(1);
			tournament.RegistrateTeam(2);
			DateTime startAt = new(2026, 4, 29);

			tournament.Start(startAt);

			Assert.True(tournament.IsStarted());
		}
		[Fact]
		public void Finish_WhenStartedAndNoUnfinishedMatches_ChangesStatusToFinished()
		{
			var tournament = new Tournament("KHL test");
			var rules = TournamentRules.CreateKHLRules();
			tournament.SetRules(rules);
			tournament.Registration();
			tournament.RegistrateTeam(1);
			tournament.RegistrateTeam(2);
			DateTime startAt = new(2026, 4, 29);
			tournament.Start(startAt);
			DateTime finishAt = new(2026, 04, 29, 14, 00, 00);
			DateTime lastMatchFinishedAt = new(2026, 04, 29, 13, 55, 00);

			tournament.Finish(finishAt, unfinishedMatchesCount: 0, lastMatchFinishedAt);

			Assert.True(tournament.IsFinished());
			Assert.Equal(finishAt, tournament.FinishedAt);
		}
		[Fact]
		public void Finish_WithUnfinishedMatches_ThrowsArgumentException()
		{
			var tournament = new Tournament("KHL test");
			var rules = TournamentRules.CreateKHLRules();
			tournament.SetRules(rules);
			tournament.Registration();
			tournament.RegistrateTeam(1);
			tournament.RegistrateTeam(2);
			DateTime startAt = new(2026, 4, 29);
			tournament.Start(startAt);
			DateTime finishAt = new(2026, 04, 29, 14, 00, 00);
			DateTime lastMatchFinishedAt = new(2026, 04, 29, 13, 55, 00);

			var ex = Assert.Throws<DomainException>(() =>
				tournament.Finish(
					finishAt: finishAt,
					unfinishedMatchesCount: 1,
					lastMatchFinishedAt: lastMatchFinishedAt));

			Assert.Equal(TournamentError.TournamentCannotBeFinishedWithUnfinishedMatches.Code, ex.Code);
		}
	}
}