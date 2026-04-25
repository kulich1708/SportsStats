using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Tournaments.DTOs.Requests
{
	public record TournamentGeneralInfoDTO(string Name, byte[]? Photo, string? PhotoMime);
}
