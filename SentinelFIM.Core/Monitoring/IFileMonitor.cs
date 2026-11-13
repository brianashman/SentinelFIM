using System;
using System.Collections.Generic;
using System.Text;

namespace SentinelFIM.Core.Monitoring
{
	public interface IFileMonitor
	{
		Task StartAsync(string path, CancellationToken cancellationToken, int intervalSeconds = 5);
		Task StopAsync();
	}
}
