using SportsStats.Application.Players.DTOs.Responses;
using SportsStats.Domain.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Players
{
	public static class PlayerMapper
	{
		public static PlayerDTO ToDTO(Player player, string teamName, string position)
			=> new(player.Id, player.Name, player.Surname, player.TeamId, teamName, position);
	}
}
