using SentinelFIM.Core.File;

namespace SentinelFIM.Infrastructure.File
{
	public class FileSystemProvider : IFileProvider
	{
		public IEnumerable<string> EnumeratePaths(string path, SearchOption searchOption)
		{
			throw new NotImplementedException();
		}

		public Stream OpenRead(string path)
		{
			throw new NotImplementedException();
		}
	}
}
