using SentinelFIM.Core.Identifiers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SentinelFIM.Core.File
{
	public interface IFileProvider
	{
		string FileProvider { get; }
		IEnumerable<string> EnumeratePaths(string path, string EnumerationSearchOption);
		Stream OpenRead(string path);
	}
}
