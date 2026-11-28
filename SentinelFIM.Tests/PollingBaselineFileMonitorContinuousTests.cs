using Microsoft.Extensions.Logging;
using Moq;
using SentinelFIM.Core.Baseline;
using SentinelFIM.Core.Hashing;
using SentinelFIM.Core.Models.Monitoring;
using SentinelFIM.Infrastructure.Baseline.Store.Json;
using SentinelFIM.Infrastructure.Hashing;
using SentinelFIM.Infrastructure.Monitoring.Baseline;
using SentinelFIM.Tests.Models;
using SentinelFIM.Tests.Shared;
using System.Diagnostics;

namespace SentinelFIM.Tests
{
	// Monitor integration tests contain duplicated setup logic.
	// Consider extracting common setup if additional monitor tests are added.
	public class PollingBaselineFileMonitorContinuousTests
	{
		private readonly Mock<ILogger<PollingBaselineFileMonitor>> _pollingLoggerMock;
		private readonly Mock<ILogger<JsonBaselineReader>> _readerLoggerMock;

		private readonly IHashProvider _hashProvider;
		private readonly IBaselineReader _baselineReader;
		private readonly TimeSpan _timeout = TimeSpan.FromSeconds(10);

		private const int _intervalSeconds = 2; // Interval for polling in seconds
		private const int _modificationCount = 3; // Number of modifications to perform during the test

		private readonly PollingBaselineFileMonitor _baselineFileMonitor;
		public PollingBaselineFileMonitorContinuousTests()
		{
			_pollingLoggerMock = new Mock<ILogger<PollingBaselineFileMonitor>>();
			_readerLoggerMock = new Mock<ILogger<JsonBaselineReader>>();

			_hashProvider = new SHA256HashProvider();
			_baselineReader = new JsonBaselineReader(_readerLoggerMock.Object);
			_baselineFileMonitor = new PollingBaselineFileMonitor(_pollingLoggerMock.Object, _hashProvider, _baselineReader);
		}

		//Private methods for PollingBaselineFileMonitor constitute the internal class implementation and will not be tested.
		//Each method has its own cts and test directories to ensure isolation and avoid interference between tests.


		[Fact]
		public async Task Monitor_DetectsModifications_RootBaselineFile()
		{
			using var cts = new CancellationTokenSource(_timeout);

			//Assumes that the monitor will detect changes within the timeout period.
			//If the timeout is too short, the test may fail due to timing issues rather than actual functionality. 
			var testDirectoryPath = String.Empty;
			try
			{
				var environment = MonitoringTestUtilities.CreateTestEnvironment(SearchOption.TopDirectoryOnly, 
					BaselineCreationOption.RootOnly, cts.Token);

				testDirectoryPath = environment;

				var baselineFilePath = MonitoringTestUtilities.GetCurrentDirectoryBaselinePath(testDirectoryPath);
				bool monitorStarted = false;

				// Wait until event fires before modifying files to ensure the monitor is actively monitoring
				_baselineFileMonitor.StatusChanged += (_, status) =>
				{
					if (status.Status == MonitoringStatusType.Started) monitorStarted = true;
				};
				var monitorTask = Task.Run(() =>
				{
					return _baselineFileMonitor.Monitor(baselineFilePath, _intervalSeconds, cts.Token)
						.Select(r => Path.GetFileName(r.Path)).Distinct()
						.Take(_modificationCount)
						.ToHashSet();
				}, cts.Token);

				while (!monitorStarted)
				{
					await Task.Delay(50, cts.Token); // Wait for the monitor to start
				}

				var modifiedFiles = MonitoringTestUtilities.ModifyTestEnvironment(testDirectoryPath, SearchOption.TopDirectoryOnly, _modificationCount, isRandomCount: false);
				
				var expectedResults = modifiedFiles.Select(file => Path.GetFileName(file)).ToHashSet();
				
				var actualResults = await monitorTask;

				//Assert that the scan results contain the expected modified files
				Assert.Equal(expectedResults, actualResults);
			}
			finally
			{
				//Ensure CancellationTokenSource is canceled to stop the monitor and clean up resources
				FileSystemTestUtilities.DeleteTestDirectoryRecursive(testDirectoryPath);
			}
		}

		[Fact]
		public async Task Monitor_DetectsModifications_RecursiveBaselineFile()
		{
			using var cts = new CancellationTokenSource(_timeout);
			var testDirectoryPath = String.Empty;
			try
			{
				var environment = MonitoringTestUtilities.CreateTestEnvironment(SearchOption.AllDirectories,
					BaselineCreationOption.RecursiveOnly, cts.Token);
				testDirectoryPath = environment;
				var baselineFilePath = MonitoringTestUtilities.GetRecursiveDirectoryBaselinePath(testDirectoryPath);
				bool monitorStarted = false;
				// Wait until event fires before modifying files to ensure the monitor is actively monitoring
				_baselineFileMonitor.StatusChanged += (_, status) =>
				{
					if (status.Status == MonitoringStatusType.Started) monitorStarted = true;
				};
				var monitorTask = Task.Run(() =>
				{
					return _baselineFileMonitor.Monitor(baselineFilePath, _intervalSeconds, cts.Token)
						.Select(r => Path.GetFileName(r.Path)).Distinct()
						.Take(_modificationCount)
						.ToHashSet();
				}, cts.Token);
				while (!monitorStarted)
				{
					await Task.Delay(50, cts.Token); // Wait for the monitor to start
				}
				var modifiedFiles = MonitoringTestUtilities.ModifyTestEnvironment(testDirectoryPath, SearchOption.AllDirectories, _modificationCount, isRandomCount: false);
				var expectedResults = modifiedFiles.Select(file => Path.GetFileName(file)).ToHashSet();
				var actualResults = await monitorTask;
				//Assert that the scan results contain the expected modified files
				Assert.Equal(expectedResults, actualResults);
			}
			finally
			{
				FileSystemTestUtilities.DeleteTestDirectoryRecursive(testDirectoryPath);
			}
		}

		[Fact]
		public async Task Monitor_DetectsDeletions_RootBaselineFile()
		{
			using var cts = new CancellationTokenSource(_timeout);
			var testDirectoryPath = String.Empty;
			try
			{
				var environment = MonitoringTestUtilities.CreateTestEnvironment(SearchOption.TopDirectoryOnly,
					BaselineCreationOption.RootOnly, cts.Token);
				testDirectoryPath = environment;
				var baselineFilePath = MonitoringTestUtilities.GetCurrentDirectoryBaselinePath(testDirectoryPath);
				bool monitorStarted = false;

				_baselineFileMonitor.StatusChanged += (_, status) =>
				{
					if (status.Status == MonitoringStatusType.Started) monitorStarted = true;
				};
				var monitorTask = Task.Run(() =>
				{
					return _baselineFileMonitor.Monitor(baselineFilePath, _intervalSeconds, cts.Token)
						.Select(r => Path.GetFileName(r.Path)).Distinct()
						.Take(_modificationCount)
						.ToHashSet();
				}, cts.Token);
				while (!monitorStarted)
				{
					await Task.Delay(50, cts.Token); // Wait for the monitor to start
				}
				var deletedFiles = MonitoringTestUtilities.DeleteFilesFromTestEnvironment(testDirectoryPath, SearchOption.TopDirectoryOnly, _modificationCount, isRandomCount: false);
				var expectedResults = deletedFiles.Select(file => Path.GetFileName(file)).ToHashSet();
				var actualResults = await monitorTask;
				Assert.Equal(expectedResults, actualResults);
			}
			finally
			{
				FileSystemTestUtilities.DeleteTestDirectoryRecursive(testDirectoryPath);
			}
		}
		[Fact]
		public async Task Monitor_DetectsDeletions_RecursiveBaselineFile()
		{
			using var cts = new CancellationTokenSource(_timeout);
			var testDirectoryPath = String.Empty;
			try
			{
				var environment = MonitoringTestUtilities.CreateTestEnvironment(SearchOption.AllDirectories,
					BaselineCreationOption.RecursiveOnly, cts.Token);
				testDirectoryPath = environment;
				var baselineFilePath = MonitoringTestUtilities.GetRecursiveDirectoryBaselinePath(testDirectoryPath);
				bool monitorStarted = false;
				_baselineFileMonitor.StatusChanged += (_, status) =>
				{
					if (status.Status == MonitoringStatusType.Started) monitorStarted = true;
				};
				var monitorTask = Task.Run(() =>
				{
					return _baselineFileMonitor.Monitor(baselineFilePath, _intervalSeconds, cts.Token)
						.Select(r => Path.GetFileName(r.Path)).Distinct()
						.Take(_modificationCount)
						.ToHashSet();
				}, cts.Token);
				while (!monitorStarted)
				{
					await Task.Delay(50, cts.Token); // Wait for the monitor to start
				}
				var deletedFiles = MonitoringTestUtilities.DeleteFilesFromTestEnvironment(testDirectoryPath, SearchOption.AllDirectories, _modificationCount, isRandomCount: false);
				var expectedResults = deletedFiles.Select(file => Path.GetFileName(file)).ToHashSet();
				var actualResults = await monitorTask;
				Assert.Equal(expectedResults, actualResults);
			}
			finally
			{
				FileSystemTestUtilities.DeleteTestDirectoryRecursive(testDirectoryPath);
			}
		}
		
		[Fact]
		public async Task Monitor_Stops_WhenCancellationTokenIsCancelled()
		{
			using var cts = new CancellationTokenSource(_timeout);
			var testDirectoryPath = String.Empty;
			try
			{
				var environment = MonitoringTestUtilities.CreateTestEnvironment(SearchOption.TopDirectoryOnly,
					BaselineCreationOption.RootOnly, cts.Token);
				testDirectoryPath = environment;
				var baselineFilePath = MonitoringTestUtilities.GetCurrentDirectoryBaselinePath(testDirectoryPath);
				bool monitorStarted = false;
				_baselineFileMonitor.StatusChanged += (_, status) =>
				{
					if (status.Status == MonitoringStatusType.Started) monitorStarted = true;
				};

				// Cancel the token after starting the monitor
				var monitorTask = Task.Run(() =>
				{
					return _baselineFileMonitor.Monitor(baselineFilePath, _intervalSeconds, cts.Token)
						.Select(r => Path.GetFileName(r.Path)).Distinct()
						.Take(_modificationCount)
						.ToHashSet();
				}, cts.Token);

				while (!monitorStarted)
				{
					await Task.Delay(50, cts.Token); // Wait for the monitor to start
				}
				bool hasStopped = false;
				_baselineFileMonitor.StatusChanged += (_, status) =>
				{
					if (status.Status == MonitoringStatusType.Stopped) hasStopped = true;
				};

				cts.Cancel(); // Cancel the token to simulate cancellation during monitoring
				await monitorTask;
				Assert.True(hasStopped);
			}
			finally
			{
				FileSystemTestUtilities.DeleteTestDirectoryRecursive(testDirectoryPath);
			}
		}

		[Fact]
		public void Monitor_ThrowsException_WhenCancellationTokenIsCancelledInitially()
		{
			using var cts = new CancellationTokenSource(_timeout);
			var testDirectoryPath = String.Empty;
			try
			{
				var environment = MonitoringTestUtilities.CreateTestEnvironment(SearchOption.TopDirectoryOnly,
					BaselineCreationOption.RootOnly, cts.Token);
				testDirectoryPath = environment;
				var baselineFilePath = MonitoringTestUtilities.GetCurrentDirectoryBaselinePath(testDirectoryPath);
				// Cancel the token before starting the monitor
				cts.Cancel();
				Assert.Throws<OperationCanceledException>(() =>
				{
					_baselineFileMonitor.Monitor(baselineFilePath, _intervalSeconds, cts.Token)
						.Select(r => Path.GetFileName(r.Path)).Distinct()
						.Take(_modificationCount)
						.ToHashSet();
				});
			}
			finally
			{
				FileSystemTestUtilities.DeleteTestDirectoryRecursive(testDirectoryPath);
			}
		}
	}
}
