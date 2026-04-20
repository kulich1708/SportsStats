using System;
using System.Collections.Generic;
using System.Text;
using SportsStats.Application.Players.DTOs.Shared;

namespace SportsStats.Application.Players.DTOs.Responses
{
	public record PlayerDTO(
		int Id, string Name, string Surname,
		int? TeamId, string? TeamName,
		string Position, byte[]? Photo, string? PhotoMime,
		CitizenshipDTO? Citizenship
	);
}
