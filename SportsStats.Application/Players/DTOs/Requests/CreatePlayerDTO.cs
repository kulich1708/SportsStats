using SportsStats.Domain.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Players.DTOs.Requests
{
	public record CreatePlayerDTO(string Name, string Surname, PositionType Position);
}
