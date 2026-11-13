using Microsoft.Extensions.Logging;
using SentinelFIM.Core.Monitoring;

namespace SentinelFIM.Infrastructure.Monitoring
{
	public class PollingFileMonitor : IFileMonitor
	{
		private readonly ILogger<PollingFileMonitor> _logger;
		//private readonly Dictionary<string, > _inMemoryBaseline;
		//Reminder to increase the intervalSeconds to a more reasonable value for release.
		public Task StartAsync(string path, CancellationToken cancellationToken, int intervalSeconds = 5)
		{
			throw new NotImplementedException();
		}

		public Task StopAsync()
		{
			throw new NotImplementedException();
		}
	}
}
