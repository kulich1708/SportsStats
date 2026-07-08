using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Players
{
	public static class PlayerError
	{
		// ===== 4100-4109: Имя игрока =====
		public static readonly ErrorCode FirstNameCannotBeEmpty = new(4100, "Имя не может быть пустым");
		public static readonly ErrorCode LastNameCannotBeEmpty = new(4101, "Фамилия не может быть пустой");
		public static readonly ErrorCode FirstNameTooLong = new(4102, "Имя не может превышать 20 символов");
		public static readonly ErrorCode LastNameTooLong = new(4103, "Фамилия не может превышать 20 символов");

		// ===== 4110-4119: Номер игрока =====
		public static readonly ErrorCode PlayerNumberOutOfRange = new(4110, "Номер игрока должен быть в диапазоне от 1 до 99");
	}
}
