using SportsStats.Domain.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Players.DTOs.Responses
{
	public record PlayerPositionDTO(PositionType Code, string Name);
}
