using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Players
{
	public class PlayerPosition<TPositionsType>
	{
		public TPositionsType PrimaryPosition { get; private set; }
		public HashSet<TPositionsType> Positions { get; private set; }

		public PlayerPosition(TPositionsType primaryPosition)
		{
			SetPrimaryPosition(primaryPosition);
		}
		public void SetPrimaryPosition(TPositionsType primaryPosition)
		{
			PrimaryPosition = primaryPosition;
		}
		public void AddPosition(TPositionsType position)
		{
			Positions.Add(position);
		}
		public void RemovePosition(TPositionsType position)
		{
			Positions.Remove(position);
		}
		public void ClearPositions()
		{
			Positions.Clear();
		}
		public void SetPositions(IEnumerable<TPositionsType> positions)
		{
			Positions = positions.ToHashSet();
		}

	}
}
