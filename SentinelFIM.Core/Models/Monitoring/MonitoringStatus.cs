using System;
using System.Collections.Generic;
using System.Text;

namespace SentinelFIM.Core.Models.Monitoring
{
	public record MonitoringStatus(MonitoringStatusType Status, string Message, DateTime TimeStamp);
}
