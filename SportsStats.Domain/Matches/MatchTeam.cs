using SportsStats.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Matches
{
	public class MatchTeam(int id)
	{
		private readonly HashSet<int> _roster = new();
		public int Id { get; private set; } = id;
		public IReadOnlySet<int> Roster => _roster;
		public int Score { get; private set; } = 0;
		public MatchWinType WinType { get; private set; }

		internal void SetWinType(MatchWinType winType) => WinType = winType;

		internal bool IsPlayerInRoster(int playerId)
		{
			return Roster.Contains(playerId);
		}
		internal void AddGoal() => Score++;
		internal void AddToRoster(int playerId) => _roster.Add(playerId);
		internal void ClearRoster() => _roster.Clear();

	}
}
