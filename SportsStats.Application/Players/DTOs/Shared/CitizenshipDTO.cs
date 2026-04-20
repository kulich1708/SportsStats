using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Players.DTOs.Shared
{
	public record CitizenshipDTO(string Name, byte[]? Photo, string? PhotoMime);
}
