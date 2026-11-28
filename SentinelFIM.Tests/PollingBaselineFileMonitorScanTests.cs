using Microsoft.Extensions.Logging;
using Moq;
using SentinelFIM.Core.Baseline;
using SentinelFIM.Core.Hashing;
using SentinelFIM.Core.Monitoring.Baseline;
using SentinelFIM.Infrastructure.Baseline.Store.Json;
using SentinelFIM.Infrastructure.Hashing;
using SentinelFIM.Infrastructure.Monitoring.Baseline;
using SentinelFIM.Tests.Models;
using SentinelFIM.Tests.Shared;
using System.Text.Json;

namespace SentinelFIM.Tests
{
	public class PollingBaselineFileMonitorScanTests
	{
		private readonly Mock<ILogger<PollingBaselineFileMonitor>> _monitorLoggerMock;
		private readonly Mock<ILogger<JsonBaselineReader>> _readerLoggerMock;

		private readonly IBaselineReader _baselineReader;
		private readonly IHashProvider _hashProvider;
		private readonly TimeSpan _timeout = TimeSpan.FromSeconds(10);

		private readonly PollingBaselineFileMonitor _baselineFileMonitor;


		public PollingBaselineFileMonitorScanTests()
		{
			_monitorLoggerMock = new Mock<ILogger<PollingBaselineFileMonitor>>();
			_readerLoggerMock = new Mock<ILogger<JsonBaselineReader>>();
			_hashProvider = new SHA256HashProvider();
			_baselineReader = new JsonBaselineReader(_readerLoggerMock.Object);
			_baselineFileMonitor = new PollingBaselineFileMonitor(_monitorLoggerMock.Object, _hashProvider, _baselineReader);
		}

		private void DisposeTestEnvironment(string directory) => FileSystemTestUtilities.DeleteTestDirectoryRecursive(directory);

		//Private methods for PollingBaselineFileMonitor constitute the internal class implementation and will not be tested.
		//Each method has its own cts and test directories to ensure isolation and avoid interference between tests.
		
		[Fact]
		public void Scan_DetectsModifications_RootBaselineFile()
		{
			using var cts = new CancellationTokenSource(_timeout);
			var testDirectoryPath = String.Empty;
			const SearchOption searchOption = SearchOption.TopDirectoryOnly;
			try
			{
				var environment = MonitoringTestUtilities.CreateTestEnvironment(searchOption, BaselineCreationOption.RootOnly, TestOperation.Modify, cts.Token);
				testDirectoryPath = environment.tempEnvPath;

				var filesChanged = environment.modifiedFiles;
				var baselineFilePath = MonitoringTestUtilities.GetCurrentDirectoryBaselinePath(testDirectoryPath);

				var expectedResults = filesChanged.Select(file => Path.GetFileName(file)).ToHashSet();
				var scanResults = _baselineFileMonitor.Scan(baselineFilePath, cts.Token)
					.Select(r => Path.GetFileName(r.Path))
					.ToHashSet();
				//Assert that the scan results contain the expected modified files
				Assert.Equal(expectedResults, scanResults);
			}
			finally
			{
				DisposeTestEnvironment(testDirectoryPath);
			}
		}

		[Fact]
		public void Scan_DetectsModifications_RecursiveBaselineFile()
		{
			using var cts = new CancellationTokenSource(_timeout);
			string testDirectoryPath = String.Empty;
			const SearchOption searchOption = SearchOption.AllDirectories;
			try
			{
				var environment = MonitoringTestUtilities.CreateTestEnvironment(searchOption, BaselineCreationOption.RecursiveOnly, TestOperation.Modify, cts.Token);
				testDirectoryPath = environment.tempEnvPath;

				var filesChanged = environment.modifiedFiles;
				var baselineFilePath = MonitoringTestUtilities.GetRecursiveDirectoryBaselinePath(testDirectoryPath);

				var expectedResults = filesChanged.Select(file => Path.GetFileName(file)).ToHashSet();
				var scanResults = _baselineFileMonitor.Scan(baselineFilePath, cts.Token)
					.Select(r => Path.GetFileName(r.Path))
					.ToHashSet();

				//Assert that the scan results contain the expected modified files
				Assert.Equal(expectedResults, scanResults);

			}
			finally
			{
				DisposeTestEnvironment(testDirectoryPath);
			}
		}
		[Fact]
		public void Scan_DetectsDeletions_RootBaselineFile()
		{
			using var cts = new CancellationTokenSource(_timeout);
			var testDirectoryPath = String.Empty;
			const SearchOption searchOption = SearchOption.TopDirectoryOnly;
			try
			{
				var environment = MonitoringTestUtilities.CreateTestEnvironment(searchOption, BaselineCreationOption.RootOnly, TestOperation.Delete, cts.Token);
				
				testDirectoryPath = environment.tempEnvPath;
				var filesChanged = environment.modifiedFiles;

				var baselineFilePath = MonitoringTestUtilities.GetCurrentDirectoryBaselinePath(testDirectoryPath);
				


				var expectedResults = filesChanged.Select(file => Path.GetFileName(file)).ToHashSet();
				var scanResults = _baselineFileMonitor.Scan(baselineFilePath, cts.Token)
					.Select(r => Path.GetFileName(r.Path))
					.ToHashSet();
				//Assert that the scan results contain the expected deleted files
				Assert.Equal(expectedResults, scanResults);
			}
			finally
			{
				DisposeTestEnvironment(testDirectoryPath);
			}
		}
		[Fact]
		public void Scan_DetectsDeletions_RecursiveBaselineFile()
		{
			using var cts = new CancellationTokenSource(_timeout);
			var testDirectoryPath = String.Empty;
			const SearchOption searchOption = SearchOption.AllDirectories;
			try
			{
				var environment = MonitoringTestUtilities.CreateTestEnvironment(searchOption, BaselineCreationOption.RecursiveOnly, TestOperation.Delete, cts.Token);

				testDirectoryPath = environment.tempEnvPath;
				var filesChanged = environment.modifiedFiles;

				var baselineFilePath = MonitoringTestUtilities.GetRecursiveDirectoryBaselinePath(testDirectoryPath);

				var expectedResults = filesChanged.Select(file => Path.GetFileName(file)).ToHashSet();
				var scanResults = _baselineFileMonitor.Scan(baselineFilePath, cts.Token)
					.Select(r => Path.GetFileName(r.Path))
					.ToHashSet();
				//Assert that the scan results contain the expected deleted files
				Assert.Equal(expectedResults, scanResults);
			}
			finally
			{
				DisposeTestEnvironment(testDirectoryPath);
			}
		}

		[Fact]
		public void Scan_ThrowsException_WhenBaselineFileDoesNotExist()
		{
			using var cts = new CancellationTokenSource(_timeout);
			var testDirectoryPath = String.Empty;
			try
			{
				testDirectoryPath = FileSystemTestUtilities.CreateRandomTestDirectory();
				var nonExistentBaselineFilePath = Path.Combine(testDirectoryPath, "nonexistent_baseline.yml");

				//Force evaluation of the IEnumerable to trigger the exception using ToList()
				Assert.Throws<FileNotFoundException>(() => _baselineFileMonitor.Scan(nonExistentBaselineFilePath, cts.Token).ToList()); 
			}
			finally
			{
				FileSystemTestUtilities.DeleteTestDirectoryRecursive(testDirectoryPath);
			}
		}

		[Fact]
		public void Scan_ThrowsException_WhenBaselineFileIsInvalid()
		{
			using var cts = new CancellationTokenSource(_timeout);
			var testDirectoryPath = String.Empty;
			try
			{
				testDirectoryPath = FileSystemTestUtilities.CreateRandomTestDirectory();
				var invalidBaselineFilePath = Path.Combine(testDirectoryPath, "invalid_baseline.txt");
				File.WriteAllText(invalidBaselineFilePath, "This is not a valid baseline file.");
				//Force evaluation of the IEnumerable to trigger the exception using ToList()
				Assert.Throws<JsonException>(() => _baselineFileMonitor.Scan(invalidBaselineFilePath, cts.Token).ToList());
			}
			finally
			{
				FileSystemTestUtilities.DeleteTestDirectoryRecursive(testDirectoryPath);
			}
		}
		[Fact]
		public void Scan_ThrowsException_WhenBaselineFileIsEmpty()
		{
			using var cts = new CancellationTokenSource(_timeout);
			var testDirectoryPath = String.Empty;
			try
			{
				testDirectoryPath = FileSystemTestUtilities.CreateRandomTestDirectory();
				var emptyBaselineFilePath = Path.Combine(testDirectoryPath, "empty_baseline.txt");
				File.WriteAllText(emptyBaselineFilePath, string.Empty);

				//Force evaluation of the IEnumerable to trigger the exception using ToList()
				Assert.Throws<JsonException>(() => _baselineFileMonitor.Scan(emptyBaselineFilePath, cts.Token).ToList());
			}
			finally
			{
				FileSystemTestUtilities.DeleteTestDirectoryRecursive(testDirectoryPath);
			}
		}
		[Fact]
		public void Scan_ThrowsException_WhenCancellationIsRequested()
		{
			using var cts = new CancellationTokenSource();
			var testDirectoryPath = String.Empty;
			try
			{
				testDirectoryPath = FileSystemTestUtilities.CreateRandomTestDirectory();
				var baselineFilePath = Path.Combine(testDirectoryPath, "baseline.json");
				File.WriteAllText(baselineFilePath, "{}"); // Create a dummy baseline file
				//Cancel the token before calling Scan
				cts.Cancel(); 
				// Force evaluation of the IEnumerable to trigger the exception using ToList()
				Assert.Throws<OperationCanceledException>(() => _baselineFileMonitor.Scan(baselineFilePath, cts.Token).ToList());
			}
			finally
			{
				FileSystemTestUtilities.DeleteTestDirectoryRecursive(testDirectoryPath);
			}
		}
	}
}
