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
		event EventHandler<MonitoringStatus> StatusChanged;
		IEnumerable<MonitoringAlert> Scan(string path, CancellationToken cancellationToken); //Single Check
		//intervalSeconds is used by polling-based monitors and ignored by
		//implementations that do not require a polling interval.
		IEnumerable<MonitoringAlert> Monitor(string path, int? intervalSeconds, CancellationToken cancellationToken); // Continuous Check
	}
}
