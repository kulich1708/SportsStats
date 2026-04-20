using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Players.DTOs.Responses
{
	public record PlayerDTO(
		int Id, string Name, string Surname,
		int TeamId, string TeamName,
		string Position, byte[]? Photo, string? PhotoMime,
		CitizenshipDTO? Citizenship
	);
	public record CitizenshipDTO(string Name, byte[]? Photo, string? PhotoMime);
}
