using Microsoft.Extensions.Logging;
using Moq;
using SentinelFIM.Core.Models;
using SentinelFIM.Infrastructure.Files;
using System.Diagnostics;
using SentinelFIM.Tests.Shared;
namespace SentinelFIM.Tests
{
	public class FilesystemProviderTests
	{
		private readonly FileSystemProvider _filesystemProvider;
		private readonly Mock<ILogger<FileSystemProvider>> _loggerMock;
		public FilesystemProviderTests()
		{
			_loggerMock = new Mock<ILogger<FileSystemProvider>>();
			_filesystemProvider = new FileSystemProvider(_loggerMock.Object);
		}

		[Fact]
		public void EnumeratePaths_ValidDirectory_ReturnsAllFiles()
		{
			string directoryPath = string.Empty;
			try
			{

				directoryPath = FileSystemTestUtilities.CreateRandomTestDirectoryWithFiles("test1.txt", "test2.txt", "test3.txt");


				//Added subdirectory to ensure RootOnly option does not include files from subdirectories
				var subdirectoryName = "subdir";
				var subDirectoryPath = FileSystemTestUtilities.CreateSubDirectory(directoryPath, subdirectoryName);
				FileSystemTestUtilities.AddFilesToDirectory(subDirectoryPath, "test1.txt", "test2.txt", "test3.txt");

				var stringPaths = _filesystemProvider.EnumeratePaths(directoryPath, EnumerationSearchOption.RootOnly).ToHashSet();
				//var stringPaths = _filesystemProvider.EnumeratePaths(directoryPath, EnumerationSearchOption.AllDirectories).ToHashSet();

				//Assert that the enumerated paths match the expected paths
				//Manually create the expected paths to avoid relying on Directory.EnumerateFiles, due to custom logic in the FileSystemProvider that may differ from the behavior of Directory.EnumerateFiles.
				var expectedPaths = FileSystemTestUtilities.CreateExpectedPaths(directoryPath, "test1.txt", "test2.txt", "test3.txt");

				Assert.Equal(expectedPaths, stringPaths);
			}
			finally
			{
				FileSystemTestUtilities.DeleteTestDirectoryRecursive(directoryPath);
			}
		}

		[Fact]
		public void EnumeratePaths_RecursiveEnumeration_ReturnsAllFilesInSubdirectories()
		{
			var directoryPath = string.Empty;
			try
			{
				directoryPath = FileSystemTestUtilities.CreateRandomTestDirectoryWithFiles("test1.txt", "test2.txt", "test3.txt", "test4.txt");
				
				var subdirectoryName = "subdir";
				var subDirectoryPath = FileSystemTestUtilities.CreateSubDirectory(directoryPath, subdirectoryName);
				
				FileSystemTestUtilities.AddFilesToDirectory(subDirectoryPath, "test1.txt", "test2.txt", "test3.txt");


				var stringPaths = _filesystemProvider.EnumeratePaths(directoryPath, EnumerationSearchOption.AllDirectories).ToHashSet();

				//Assert that the enumerated paths match the expected paths
				//Manually create the expected paths to avoid relying on Directory.EnumerateFiles, due to custom logic in the FileSystemProvider that may differ from the behavior of Directory.EnumerateFiles.

				
				var expectedPaths = FileSystemTestUtilities.CreateExpectedPaths(directoryPath, "test1.txt", "test2.txt", "test3.txt", "test4.txt");
				var expectedSubdirectoryPaths = FileSystemTestUtilities.CreateExpectedPaths(subDirectoryPath, "test1.txt", "test2.txt", "test3.txt");

				var combinedExpectedPaths = new HashSet<string>(expectedPaths);
				combinedExpectedPaths.UnionWith(expectedSubdirectoryPaths);

				Assert.Equal(combinedExpectedPaths, stringPaths);
			}
			finally
			{
				FileSystemTestUtilities.DeleteTestDirectoryRecursive(directoryPath);
			}
		}
		[Fact]
		public void EnumeratePaths_EmptyDirectory_ReturnsEmptyCollection()
		{
			var directoryPath = string.Empty;
			try
			{
				directoryPath = FileSystemTestUtilities.CreateRandomTestDirectory();
					
				var stringPaths = _filesystemProvider.EnumeratePaths(directoryPath, EnumerationSearchOption.RootOnly).ToHashSet();

				//Assert that enumerated paths return an empty collection
				Assert.Empty(stringPaths);
			}
			finally
			{
				FileSystemTestUtilities.DeleteTestDirectoryRecursive(directoryPath);
			}
		}
	}
}
