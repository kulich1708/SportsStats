using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Matches.DTOs.Responses
{
	public record PeriodDTO(int Current, bool IsBreak, string? Title);
}
