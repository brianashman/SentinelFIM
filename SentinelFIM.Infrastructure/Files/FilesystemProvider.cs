using Microsoft.Extensions.Logging;
using SentinelFIM.Core.Files;
using SentinelFIM.Core.Identifiers;
using SentinelFIM.Core.Models;
using System.Diagnostics;

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
		private IEnumerable<string> EnumerateRecursive(string path)
		{
			//Enumerates files in current directory
			IEnumerable<string> files;
			try
			{
				files = Directory.EnumerateFiles(path);
			}
			catch (UnauthorizedAccessException ex)
			{
				_logger.LogWarning("Access denied to directory: {path}. Error: {error}", path, ex.Message);
				yield break;
			}
			foreach (var file in files)
			{
				yield return file;
			}
			//Enumerates subdirectories in current directory
			IEnumerable<string> directories;
			try
			{
				directories = Directory.EnumerateDirectories(path);
			}
			catch (UnauthorizedAccessException ex)
			{
				_logger.LogWarning("Access denied to directory: {path}. Error: {error}", path, ex.Message);
				yield break;
			}
			foreach (var directory in directories)
			{
				foreach (var file in EnumerateRecursive(directory))
				{
					yield return file;
				}
			}
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
					EnumerationSearchOption.AllDirectories => EnumerateRecursive(path),
					EnumerationSearchOption.RootOnly => Directory.EnumerateFiles(path, "*", SearchOption.TopDirectoryOnly),
					_ => throw new ArgumentOutOfRangeException(nameof(searchOption), searchOption, null),
				};
			}

			catch (UnauthorizedAccessException ex)
			{
				_logger.LogWarning("Access denied to directory: {path}. Error: {error}", path, ex.Message);
				throw;
			}
			catch (PathTooLongException ex)
			{
				_logger.LogError("The specified path is too long: {path}. Error: {error}", path, ex.Message);
				throw;
			}
			catch (IOException ex)
			{
				_logger.LogError("An I/O error occurred while accessing the directory: {path}. Error: {error}", path, ex.Message);
				throw;
			}
		}
		public Stream OpenRead(string path) => File.OpenRead(path);
	}
}
