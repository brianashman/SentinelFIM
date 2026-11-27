using Microsoft.Extensions.Logging;
using Moq;
using SentinelFIM.Core.Baseline;
using SentinelFIM.Core.Hashing;
using SentinelFIM.Core.Monitoring.Baseline;
using SentinelFIM.Infrastructure.Baseline.Store.Json;
using SentinelFIM.Infrastructure.Hashing;
using SentinelFIM.Infrastructure.Monitoring.Baseline;
using SentinelFIM.Tests.Shared;

namespace SentinelFIM.Tests
{
	public class PollingBaseLineFileMonitorTests
	{
		private readonly Mock<ILogger<PollingBaselineFileMonitor>> _monitorLoggerMock;
		private readonly Mock<ILogger<JsonBaselineReader>> _readerLoggerMock;

		private readonly IBaselineReader _baselineReader;
		private readonly IHashProvider _hashProvider;

		private readonly PollingBaselineFileMonitor _baselineFileMonitor;


		public PollingBaseLineFileMonitorTests()
		{
			_monitorLoggerMock = new Mock<ILogger<PollingBaselineFileMonitor>>();
			_readerLoggerMock = new Mock<ILogger<JsonBaselineReader>>();
			_hashProvider = new SHA256HashProvider();
			_baselineReader = new JsonBaselineReader(_readerLoggerMock.Object);
			_baselineFileMonitor = new PollingBaselineFileMonitor(_monitorLoggerMock.Object, _hashProvider, _baselineReader);
		}


		private void DisposeTestEnvironment(string directory) => FileSystemTestUtilities.DeleteTestDirectoryRecursive(directory);
		private (string tempEnvPath, HashSet<string> modifiedFiles) CreateScanEnvironment(SearchOption searchOption, int fileCount = 3)
		{
			// Target directory for the test environment
			var testDirectoryPath = FileSystemTestUtilities.CreateRandomTestDirectory(); 

			// Existing Test Environment Paths
			var testEnvironmentPaths = MonitoringTestUtilities.GetTestEnvironment();

			// Copies the test environment to the temporary directory for testing
			MonitoringTestUtilities.CopyTestEnvironmentToTemp(testEnvironmentPaths, testDirectoryPath);

			// Modifies the test environment to simulate changes for testing purposes
			var filesChanged = MonitoringTestUtilities.ModifyTestEnvironment(testDirectoryPath, searchOption, fileCount);

			// Path to the temporary test environment for scanning & the set of modified files for validation
			return (testDirectoryPath, filesChanged); 
		}
		//Private methods constitute the internal class implementation and will not be tested.
		//Scan method constitutes the majority of the class implementation and will be prioritized for testing over continuous monitoring.
		[Fact]
		public void Scan_DetectsChanges_RootBaselineFile()
		{
			var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
			var testDirectoryPath = String.Empty;
			const SearchOption searchOption = SearchOption.TopDirectoryOnly;
			try
			{
				var environment = CreateScanEnvironment(searchOption);
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
		public void Scan_DetectsChanges_RecursiveBaselineFile()
		{
			var cts = new CancellationTokenSource();
			string testDirectoryPath = String.Empty;
			const SearchOption searchOption = SearchOption.AllDirectories;
			try
			{
				var environment = CreateScanEnvironment(searchOption);
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
	}
}
