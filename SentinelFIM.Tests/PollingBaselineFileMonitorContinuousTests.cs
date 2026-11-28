using Microsoft.Extensions.Logging;
using Moq;
using SentinelFIM.Core.Baseline;
using SentinelFIM.Core.Hashing;
using SentinelFIM.Infrastructure.Baseline.Store.Json;
using SentinelFIM.Infrastructure.Hashing;
using SentinelFIM.Infrastructure.Monitoring.Baseline;
using SentinelFIM.Tests.Models;
using SentinelFIM.Tests.Shared;

namespace SentinelFIM.Tests
{
	public class PollingBaselineFileMonitorContinuousTests
	{
		private readonly Mock<ILogger<PollingBaselineFileMonitor>> _pollingLoggerMock;
		private readonly Mock<ILogger<JsonBaselineReader>> _readerLoggerMock;

		private readonly IHashProvider _hashProvider;
		private readonly IBaselineReader _baselineReader;
		private readonly TimeSpan _timeout = TimeSpan.FromSeconds(10);

		private readonly PollingBaselineFileMonitor _monitor;
		public PollingBaselineFileMonitorContinuousTests()
		{
			_pollingLoggerMock = new Mock<ILogger<PollingBaselineFileMonitor>>();
			_readerLoggerMock = new Mock<ILogger<JsonBaselineReader>>();

			_hashProvider = new SHA256HashProvider();
			_baselineReader = new JsonBaselineReader(_readerLoggerMock.Object);
			_monitor = new PollingBaselineFileMonitor(_pollingLoggerMock.Object, _hashProvider, _baselineReader);
		}

		//Private methods for PollingBaselineFileMonitor constitute the internal class implementation and will not be tested.
		//Each method has its own cts and test directories to ensure isolation and avoid interference between tests.


		[Fact]
		public void Monitor_DetectsModifications_RootBaselineFile()
		{
			
		}
	}
}
