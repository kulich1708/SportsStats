using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Application.Shared
{
	public record PaginationDTO(
		int Page,
		int PageSize
	);
}
