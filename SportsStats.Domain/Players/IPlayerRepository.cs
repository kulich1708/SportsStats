using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Players
{
	public interface IPlayerRepository
	{
		public Player FindById(int playerId);
		public Player Save(Player player);
	}
}
