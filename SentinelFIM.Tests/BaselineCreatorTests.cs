using Microsoft.Extensions.Logging;
using Moq;
using SentinelFIM.Core.Files;
using SentinelFIM.Core.Hashing;
using SentinelFIM.Core.Models;
using SentinelFIM.Infrastructure.Baseline.Creator;
using SentinelFIM.Tests.Shared;

namespace SentinelFIM.Tests
{
	public class BaselineCreatorTests : IDisposable
	{
		private readonly BaselineCreator _baselineCreator;
		private readonly Mock<IFileProvider> _fileProviderMock;
		private readonly Mock<IHashProvider> _hashProviderMock;
		private readonly Mock<ILogger<BaselineCreator>> _loggerMock;

		private readonly HashSet<string> _testPaths;
		private readonly string _testDirectoryPath;
		private readonly string _testHashAlgorithm = "SHA256"; // Default hash algorithm for testing
		private readonly string _testHashValue = "HashValue"; // Default hash value for testing
		private void Setup()
		{			
			_fileProviderMock.Setup(fp => fp.EnumeratePaths(_testDirectoryPath, It.IsAny<string>())).Returns(_testPaths);

			_hashProviderMock.Setup(h => h.Algorithm).Returns(_testHashAlgorithm);
			_hashProviderMock.Setup(h => h.ComputeHash(It.IsAny<Stream>())).Returns(_testHashValue);
		}
		
		public BaselineCreatorTests()
		{
			_loggerMock = new Mock<ILogger<BaselineCreator>>();
			_fileProviderMock = new Mock<IFileProvider>();
			_hashProviderMock = new Mock<IHashProvider>();
			
			string[] testFiles = ["file1.txt", "file2.txt", "file3.txt"];
			_testDirectoryPath = FileSystemTestUtilities.CreateRandomTestDirectoryWithFiles(testFiles);
			_testPaths = FileSystemTestUtilities.CreateExpectedPaths(_testDirectoryPath, testFiles);
			
			Setup();

			_baselineCreator = new BaselineCreator(_loggerMock.Object, _fileProviderMock.Object, _hashProviderMock.Object);
		}


		[Fact]
		public void CreateFileBaseline_ReturnsExpectedScanResult()
		{
			var fileName = "file1.txt";
			var targetPath = Path.Combine(_testDirectoryPath, fileName);
			ScanResult result = _baselineCreator.CreateFileBaseline(targetPath);

			Assert.Equal(_testHashValue, result.Hash);
			Assert.Equal(_testHashAlgorithm, result.Algorithm);
			Assert.Equal(fileName, result.FileName);
			Assert.Equal(targetPath, result.Path);

			var expectedSize = new FileInfo(targetPath).Length;
			Assert.Equal(expectedSize, result.FileSize);
		}
		//Create Directory Baseline is Separated into two cases, one for RootOnly and another for AllDirectories to ensure both search options are tested.
		//No need for parameterized tests here, there are only two cases.
		[Fact]
		public void CreateDirectoryBaseline_RootOnly_ReturnsExpectedScanResults()
		{
			var results = _baselineCreator.CreateDirectoryBaseline(_testDirectoryPath, false, CancellationToken.None).ToList();
			Assert.Equal(_testPaths.Count, results.Count);
			foreach (var result in results)
			{
				Assert.Equal(Path.GetFileName(result.Path), result.FileName);
				Assert.Equal(_testHashValue, result.Hash);
				Assert.Equal(_testHashAlgorithm, result.Algorithm);
				Assert.Contains(result.Path, _testPaths);
				var expectedSize = new FileInfo(result.Path).Length;
				Assert.Equal(expectedSize, result.FileSize);
			}
		}
		[Fact]
		public void CreateDirectoryBaseline_AllDirectories_ReturnsExpectedScanResults()
		{

			var subDirectoryName = "subdir";
			var subDirectoryPath = FileSystemTestUtilities.CreateSubDirectory(_testDirectoryPath, subDirectoryName);
			FileSystemTestUtilities.AddFilesToDirectory(subDirectoryPath, "test1.txt", "test2.txt", "test3.txt");

			_testPaths.UnionWith(FileSystemTestUtilities.CreateExpectedPaths(subDirectoryPath, "test1.txt", "test2.txt", "test3.txt"));

			var results = _baselineCreator.CreateDirectoryBaseline(_testDirectoryPath, true, CancellationToken.None).ToList();

			Assert.Equal(_testPaths.Count, results.Count);
			foreach (var result in results)
			{
				Assert.Equal(Path.GetFileName(result.Path), result.FileName);
				Assert.Equal(_testHashValue, result.Hash);
				Assert.Equal(_testHashAlgorithm, result.Algorithm);
				Assert.Contains(result.Path, _testPaths);
				var expectedSize = new FileInfo(result.Path).Length;
				Assert.Equal(expectedSize, result.FileSize);
			}
		}

		void IDisposable.Dispose() => FileSystemTestUtilities.DeleteTestDirectoryRecursive(_testDirectoryPath);
	}
}
