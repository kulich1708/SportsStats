using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Teams
{
	public static class TeamError
	{
		// ===== 4000-4009: Название команды =====
		public static readonly ErrorCode TeamNameCannotBeEmpty = new(4000, "Название команды не может быть пустым");
	}
}
