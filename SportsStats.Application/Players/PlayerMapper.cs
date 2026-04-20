using SportsStats.Application.Players.DTOs.Responses;
using SportsStats.Domain.Players;
using SportsStats.Application.Players.DTOs.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Players
{
	public static class PlayerMapper
	{
		public static PlayerDTO ToDTO(Player player, string? teamName, string position)
			=> new(player.Id, player.Name, player.Surname,
				player.TeamId, teamName, position, player.Photo, player.PhotoMime,
				player.Citizenship == null ? null : ToDTO(player.Citizenship));
		public static CitizenshipDTO ToDTO(Citizenship citizenship) => new(citizenship.Name, citizenship.Photo, citizenship.PhotoMime);
	}
}
