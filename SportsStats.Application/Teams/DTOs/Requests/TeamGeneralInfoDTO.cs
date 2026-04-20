using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Teams.DTOs.Requests
{
	public record TeamGeneralInfoDTO(
		string Name,
		string? City,
		byte[]? Photo,
		string? PhotoMime
	);
}
