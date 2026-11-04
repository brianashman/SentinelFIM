using SentinelFIM.Core.Identifiers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SentinelFIM.Core.Files
{
	public interface IFileProvider
	{
		string ProviderType { get; }
		IEnumerable<string> EnumeratePaths(string path, string searchOption);
		Stream OpenRead(string path);
	}
}
