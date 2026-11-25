using Microsoft.Extensions.Logging;
using Moq;
using SentinelFIM.Core.Baseline;
using SentinelFIM.Core.Hashing;
using SentinelFIM.Core.Monitoring.Baseline;
using SentinelFIM.Infrastructure.Monitoring.Baseline;

namespace SentinelFIM.Tests
{
	public class PollingBaseLineFileMonitorTests
	{
		private readonly Mock<ILogger<PollingBaselineFileMonitor>> _loggerMock;
		private readonly Mock<IBaselineStore> _baselineStore;
		private readonly Mock<IHashProvider> _baselineFileMonitorMock;
		private readonly string _testDirectoryPath;
		
		public PollingBaseLineFileMonitorTests()
		{
			_loggerMock = new Mock<ILogger<PollingBaselineFileMonitor>>();
			_baselineFileMonitorMock = new Mock<IHashProvider>();
			_baselineStore = new Mock<IBaselineStore>();
		}

		//Private methods constitute the internal class implementation and will not be tested.
		[Fact]
		public void Scan_DetectsChangesInBaselineFile()
		{

		}
	}
}
