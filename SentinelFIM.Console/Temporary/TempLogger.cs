using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SentinelFIM.Console.Temporary
{
	public class TempLogger
	{
		private readonly ILogger<TempLogger> _logger;
		public TempLogger(ILogger<TempLogger> logger)
		{
			_logger = logger;
		}
		public void LogInformation(string message)
		{
			_logger.LogInformation(message);
		}
		public void LogWarning(string message)
		{
			_logger.LogWarning(message);
		}
		public void LogError(string message)
		{
			_logger.LogError(message);
		}
	}
}
