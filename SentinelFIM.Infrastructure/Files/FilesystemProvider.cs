using SentinelFIM.Core.Files;
using SentinelFIM.Core.Identifiers;

namespace SentinelFIM.Infrastructure.Files
{
	public class FileSystemProvider : IFileProvider
	{
		public string ProviderType => FileProviderType.FileSystem;

		public IEnumerable<string> EnumeratePaths(string path, string searchOption)
		{

			return searchOption switch
			{
				EnumerationSearchOption.AllDirectories => Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories),
				EnumerationSearchOption.RootOnly => Directory.EnumerateFiles(path, "*", SearchOption.TopDirectoryOnly),
				_ => throw new ArgumentOutOfRangeException(nameof(searchOption), searchOption, null),
			};
		}

		public Stream OpenRead(string path) => File.OpenRead(path);
	}
}
