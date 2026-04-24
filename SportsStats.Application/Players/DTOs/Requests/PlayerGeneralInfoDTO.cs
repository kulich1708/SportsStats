using SportsStats.Domain.Players;
using SportsStats.Application.Players.DTOs.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Players.DTOs.Requests
{
	public record PlayerGeneralInfoDTO(
		string Name,
		string Surname,
		PositionType Position,
		int? TeamId,
		int? Number,
		DateOnly? Birthday,
		CitizenshipDTO? Citizenship,
		byte[]? Photo,
		string? PhotoMime
	);
}
