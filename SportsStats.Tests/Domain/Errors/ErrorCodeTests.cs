using SportsStats.Domain.Shared;
using System.Reflection;

namespace SportsStats.Tests.Domain.Errors;

public class ErrorCodeTests
{
	[Fact]
	public void AllErrorCodes_MustBeUnique()
	{
		var allCodes = new List<int>();

		var domainAssembly = typeof(DomainException).Assembly;

		var errorTypes = domainAssembly.GetTypes()
			.Where(t => t.IsClass && t.IsAbstract && t.IsSealed
						&& t.Name.EndsWith("Error"));

		foreach (var type in errorTypes)
		{
			var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

			foreach (var field in fields)
			{
				if (field.FieldType != typeof(ErrorCode))
					continue;

				var value = (ErrorCode?)field.GetValue(null);
				if (value != null)
				{
					allCodes.Add(value.Code);
				}
			}
		}

		var duplicates = allCodes
			.GroupBy(x => x)
			.Where(g => g.Count() > 1)
			.Select(g => g.Key)
			.ToList();

		Assert.Empty(duplicates);
	}
}