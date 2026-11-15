using Microsoft.Extensions.Logging;
using SentinelFIM.Core.Files;
using SentinelFIM.Core.Identifiers;
using SentinelFIM.Core.Models;

namespace SentinelFIM.Infrastructure.Files
{
	public class FileSystemProvider : IFileProvider
	{
		public string ProviderType => FileProviderType.FileSystem;
		private readonly ILogger<FileSystemProvider> _logger;
		public FileSystemProvider(ILogger<FileSystemProvider> logger)
		{
			_logger = logger;
		}

		public IEnumerable<string> EnumeratePaths(string path, string searchOption)
		{
			try
			{
				if (!Directory.Exists(path))
				{
					throw new DirectoryNotFoundException($"The specified directory '{path}' does not exist.");
				}
				
				_logger.LogInformation("Enumerating files in directory: {path} with search option: {searchOption}", path, searchOption);
				// Use a switch expression to determine the search option
				return searchOption switch
				{
					EnumerationSearchOption.AllDirectories => Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories),
					EnumerationSearchOption.RootOnly => Directory.EnumerateFiles(path, "*", SearchOption.TopDirectoryOnly),
					_ => throw new ArgumentOutOfRangeException(nameof(searchOption), searchOption, null),
				};
			}

			catch (UnauthorizedAccessException)
			{
				_logger.LogWarning("Access denied to directory: {path}", path);
				throw;
			}
			catch (PathTooLongException)
			{
				_logger.LogError("The specified path is too long: {path}", path);
				throw;
			}
			catch (IOException)
			{
				_logger.LogError("An I/O error occurred while accessing the directory: {path}", path);
				throw;
			}
		}
		public Stream OpenRead(string path) => File.OpenRead(path);
	}
}
