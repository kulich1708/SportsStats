using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Infrastructure
{

	public static class NotFoundError
	{
		public static readonly ErrorCode Tournament = new(1, "Турнир с id {0} не существует");
		public static readonly ErrorCode Match = new(2, "Матч с id {0} не существует");
		public static readonly ErrorCode Player = new(2, "Игрок с id {0} не существует");
	}
}
