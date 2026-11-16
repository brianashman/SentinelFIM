using Microsoft.Extensions.Logging;
using SentinelFIM.Core.Baseline;
using SentinelFIM.Core.Files;
using SentinelFIM.Core.Models;
using SentinelFIM.Core.Monitoring.Baseline;

namespace SentinelFIM.Infrastructure.Monitoring
{
	// Meant to be synchronous
	// Task.Run will be used as a compensating control to avoid blocking the main thread.
	// Presentation is not expected to have a large number of concurrent operations, so this should be acceptable.
	public class PollingBaselineFileMonitor : IBaselineFileMonitor
	{
		private readonly ILogger<PollingBaselineFileMonitor> _logger;
		private Dictionary<string, ScanResult> _inMemoryBaseline;
		private readonly IFileProvider _fileProvider;
		private readonly IBaselineStore _baselineStore;
		public PollingBaselineFileMonitor(ILogger<PollingBaselineFileMonitor> logger, IFileProvider fileProvider, IBaselineStore baselineStore)
		{
			_logger = logger;
			_fileProvider = fileProvider;
			_baselineStore = baselineStore;
			_inMemoryBaseline = new Dictionary<string, ScanResult>();
		}
		public void LoadBaseline(string path)
		{
			var results = _baselineStore.LoadBaseline(path);
			foreach (var item in results)
			{
				_inMemoryBaseline.Add(item.Path, item);
			}
		}

		public void Scan(string path)
		{
			throw new NotImplementedException();
		}

		//Reminder to increase the intervalSeconds to a more reasonable value for release.
		public void Start(string path, CancellationToken cancellationToken, int intervalSeconds = 5)
		{
			throw new NotImplementedException();
		}

		public void Stop()
		{
			throw new NotImplementedException();
		}
	}
}
