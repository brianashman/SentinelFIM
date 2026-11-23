using Microsoft.Extensions.Logging;
using Moq;
using SentinelFIM.Core.Models;
using SentinelFIM.Infrastructure.Files;
using System.Diagnostics;

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
			string testDirectory = string.Empty;
			try
			{
				testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
				Directory.CreateDirectory(testDirectory);

				var testFile1 = Path.Combine(testDirectory, "test1.txt");
				var testFile2 = Path.Combine(testDirectory, "test2.txt");
				var testFile3 = Path.Combine(testDirectory, "test3.txt");

				File.WriteAllText(testFile1, "Test file 1");
				File.WriteAllText(testFile2, "Test file 2");
				File.WriteAllText(testFile3, "Test file 3");

				var stringPaths = _filesystemProvider.EnumeratePaths(testDirectory, EnumerationSearchOption.RootOnly).ToHashSet();

				//All paths enumerated should exist
				foreach (var path in stringPaths)
				{
					Assert.True(File.Exists(path), $"File does not exist: {path}");
				}

				HashSet<string> expectedPaths = Directory.EnumerateFiles(testDirectory, "*", SearchOption.TopDirectoryOnly).ToHashSet();
				Assert.Equal(expectedPaths, stringPaths);
			}
			finally
			{
				if(Directory.Exists(testDirectory)) Directory.Delete(testDirectory, true);
			}
		}

		///<summary>
		///To be implemented: 
		///Test for recursive enumeration of files in subdirectories
		///</summary>
		//[Fact]
		//public void EnumeratePaths_RecursiveEnumeration_ReturnsAllFilesInSubdirectories()
		//{
		//	//Test Recursive Enumeration
		//	var testSubdirectory = Path.Combine(testDirectory, "subdir");
		//	Directory.CreateDirectory(testSubdirectory);
		//}
	}
}
