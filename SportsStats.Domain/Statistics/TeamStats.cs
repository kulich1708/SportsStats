using SportsStats.Domain.Common;
using SportsStats.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Statistics
{
	public class TeamStats(int teamId, int tournamentId) : BaseEntity
	{
		public int TeamId { get; private set; } = teamId;
		public int TournamentId { get; private set; } = tournamentId;
		public int Games { get; private set; }
		public int RegularWins { get; private set; }
		public int OTWins { get; private set; }
		public int Draws { get; private set; }
		public int OTLosses { get; private set; }
		public int RegularLosses { get; private set; }
		public int Points { get; private set; }

		public void AddOutcome(MatchWinType outcome, int points)
		{
			Games++;
			switch (outcome)
			{
				case MatchWinType.REGULATION_WIN:
					AddRegularWin();
					break;
				case MatchWinType.OT_WIN:
					AddOTWin();
					break;
				case MatchWinType.DRAW:
					AddDraw();
					break;
				case MatchWinType.OT_LOSS:
					AddOTLoss();
					break;
				case MatchWinType.REGULATION_LOSS:
					AddRegularLoss();
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(outcome));
			}
			Points += points;
		}

		private void AddRegularWin()
		{
			RegularWins++;
		}
		private void AddOTWin()
		{
			OTWins++;
		}
		private void AddDraw()
		{
			Draws++;
		}
		private void AddOTLoss()
		{
			OTLosses++;
		}
		private void AddRegularLoss()
		{
			RegularLosses++;
		}
	}
}
