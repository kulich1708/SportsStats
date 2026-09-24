using SportsStats.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Infrastructure
{
	public class NotFoundException : Exception
	{
		public int Code { get; private set; } = 0;
		public NotFoundException(ErrorCode error) : base(error.Message)
		{
			Code = error.Code;
		}
		public NotFoundException(ErrorCode error, params object[] args)
			: base(string.Format(error.Message, args.Select(a => a?.ToString() ?? null).ToArray()))
		{
			Code = error.Code;
		}
	}
}
