using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Shared
{
	public class DomainException : Exception
	{
		public int Code { get; private set; } = 0;
		public DomainException(ErrorCode error) : base(error.Message)
		{
			Code = error.Code;
		}
		public DomainException(ErrorCode error, params object[] args)
			: base(string.Format(error.Message, args.Select(a => a?.ToString() ?? null).ToArray()))
		{
			Code = error.Code;
		}
	}
}
