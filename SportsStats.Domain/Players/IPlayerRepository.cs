using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Players
{
	public interface IPlayerRepository
	{
		public Task<Player?> FindById(int playerId);
		public Task<Player> Save(Player player);
	}
}
