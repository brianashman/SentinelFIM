using System;
using System.Collections.Generic;
using System.Text;

namespace SentinelFIM.Core.Models.Monitoring
{
	public enum MonitoringStatusType
	{
		Started, 
		Stopped,
		IntervalExceeded,
		Error,
	}
}
