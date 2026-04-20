using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Matches.DTOs.Requests
{
	public record AddPlayersToRosterDTO(
		List<int> PlayerIds,
		int TeamId
		);
}
