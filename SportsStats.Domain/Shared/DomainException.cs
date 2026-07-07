using System;
using System.Collections.Generic;
using System.Text;

namespace SportsStats.Domain.Shared
{
	public class DomainException : Exception
	{
		public DomainException(string message) : base(message) { }
		public DomainException(string message, params string[] args) : base(string.Format(message, args)) { }
		public DomainException(string message, params DateTime[] args) : base(string.Format(message, args)) { }
	}
}
