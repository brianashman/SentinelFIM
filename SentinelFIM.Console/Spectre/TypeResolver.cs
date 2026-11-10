using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Text;

namespace SentinelFIM.Console.Spectre
{
	public sealed class TypeResolver(IServiceProvider provider) : ITypeResolver
	{
		public object? Resolve(Type? type) => type == null ? null : provider.GetService(type);
	}
}
