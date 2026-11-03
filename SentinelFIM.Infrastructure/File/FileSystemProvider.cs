using SentinelFIM.Core.File;
using SentinelFIM.Core.Identifiers;

namespace SentinelFIM.Infrastructure.File
{
	public class FileSystemProvider : IFileProvider
	{
		public string FileProvider => FileProviderType.FileSystem;

		public IEnumerable<string> EnumeratePaths(string path, string EnumerationSearchOption)
		{
			throw new NotImplementedException();
		}

		public Stream OpenRead(string path)
		{
			throw new NotImplementedException();
		}
	}
}
