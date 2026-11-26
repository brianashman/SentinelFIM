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

		//Private methods constitute the internal class implementation and will not be tested.
		[Fact]
		public void Scan_DetectsChangesInBaselineFile()
		{
			string testDirectoryPath = String.Empty;
			try
			{
				testDirectoryPath = FileSystemTestUtilities.CreateRandomTestDirectory();
				var testEnvironmentPaths = MonitoringTestUtilities.GetTestEnvironment();
				var result = MonitoringTestUtilities.CopyTestEnvironmentToTemp(testEnvironmentPaths, testDirectoryPath);
			}
			finally
			{
				DisposeTestEnvironment(testDirectoryPath);
			}
		}
	}
}
