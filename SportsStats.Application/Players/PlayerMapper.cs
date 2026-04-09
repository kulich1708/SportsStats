using SportsStats.Application.Players.DTOs.Responses;
using SportsStats.Domain.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Players
{
	public static class PlayerMapper
	{
		public static PlayerDTO ToDTO(Player player) => new(player.Name, player.Surname, player.TeamId, player.Position);
	}
}
