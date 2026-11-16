using System;
using System.Collections.Generic;
using System.Text;

namespace SentinelFIM.Core.Models.Monitoring
{
	public record MonitoringAlert(string Path, Enum AlertType, string AlertMessage, DateTime Timestamp);
}
