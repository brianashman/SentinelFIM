using System;
using System.Collections.Generic;
using System.Text;

namespace SentinelFIM.Core.File
{
	public interface IFileProvider
	{
		IEnumerable<string> EnumeratePaths(string path, SearchOption searchOption);
		Stream OpenRead(string path);
	}
}
