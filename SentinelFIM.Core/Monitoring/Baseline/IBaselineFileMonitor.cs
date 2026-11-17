using SentinelFIM.Core.Models.Monitoring;
using System;
using System.Collections.Generic;
using System.Text;

namespace SentinelFIM.Core.Monitoring.Baseline
{
	// TODO:
	// Add monitoring exporter factory.
	// Exporters selected by self-identifying format name.
	// Monitor optionally exports alerts as they are generated.
	public interface IBaselineFileMonitor
	{
		IEnumerable<MonitoringAlert> Scan(string path); //Single Check
		void Start(string path, CancellationToken cancellationToken, int intervalSeconds = 5); // Continuous Check
		void Stop();
	}
}
