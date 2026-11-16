using System;
using System.Collections.Generic;
using System.Text;

namespace SentinelFIM.Core.Monitoring.Baseline
{
	public interface IBaselineFileMonitor
	{
		void Scan(string path);
		void Start(string path, CancellationToken cancellationToken, int intervalSeconds = 5);
		void Stop();
	}
}
