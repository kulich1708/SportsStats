using SportsStats.Domain.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Players.DTOs.Responses
{
	public record PlayerDTO(string Name, string Surname, int TeamId, PositionType Position);
}
