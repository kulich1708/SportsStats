using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Players
{
	public record PlayerPosition
	{
		public PositionType PrimaryPosition { get; private set; }
		public HashSet<PositionType> Positions { get; private set; }

		public PlayerPosition(PositionType primaryPosition)
		{
			SetPrimaryPosition(primaryPosition);
		}
		public void SetPrimaryPosition(PositionType primaryPosition)
		{
			PrimaryPosition = primaryPosition;
		}
		public void AddPosition(PositionType position)
		{
			Positions.Add(position);
		}
		public void RemovePosition(PositionType position)
		{
			Positions.Remove(position);
		}
		public void ClearPositions()
		{
			Positions.Clear();
		}
		public void SetPositions(IEnumerable<PositionType> positions)
		{
			Positions = positions.ToHashSet();
		}

	}
}
