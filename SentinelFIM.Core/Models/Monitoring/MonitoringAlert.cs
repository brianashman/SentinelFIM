using System;
using System.Collections.Generic;
using System.Text;

namespace SentinelFIM.Core.Models.Monitoring
{
	public record MonitoringAlert(
		string Path, AlertType AlertType, 
		string AlertMessage, DateTime Timestamp, DateTime? LastModified,
		string? PreviousValue, string? CurrentValue);
}
