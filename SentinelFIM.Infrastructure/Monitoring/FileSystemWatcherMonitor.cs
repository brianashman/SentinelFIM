using SentinelFIM.Core.Monitoring;

namespace SentinelFIM.Infrastructure.Monitoring
{
	public class FileSystemWatcherMonitor : IFileMonitor
	{
		public Task StartAsync(string path, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task StopAsync()
		{
			throw new NotImplementedException();
		}
	}
}
