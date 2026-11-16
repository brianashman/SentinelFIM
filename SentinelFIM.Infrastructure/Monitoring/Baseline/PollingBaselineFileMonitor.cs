using Microsoft.Extensions.Logging;
using SentinelFIM.Core.Baseline;
using SentinelFIM.Core.Files;
using SentinelFIM.Core.Hashing;
using SentinelFIM.Core.Models;
using SentinelFIM.Core.Models.Monitoring;
using SentinelFIM.Core.Monitoring.Baseline;

namespace SentinelFIM.Infrastructure.Monitoring.Baseline
{
	///<summary>
	/// Meant to be synchronous
	/// Task.Run will be used as a compensating control to avoid blocking the main thread.
	/// Presentation is not expected to have a large number of concurrent operations, so this should be acceptable.
	///
	/// Baseline is the source of truth, not the file system. The baseline is loaded into memory and used to check for changes.
	/// Files are not enumerated individually then checked against the baseline.
	/// Instead, paths are loaded from the baseline and evaluated by the monitor.
	/// </summary>
	public class PollingBaselineFileMonitor : IBaselineFileMonitor
	{
		private readonly ILogger<PollingBaselineFileMonitor> _logger;
		private Dictionary<string, ScanResult> _inMemoryBaseline;
		private readonly IHashProvider _hashProvider;
		private readonly IBaselineStore _baselineStore;
		public PollingBaselineFileMonitor(ILogger<PollingBaselineFileMonitor> logger, IHashProvider hashProvider, IBaselineStore baselineStore)
		{
			_logger = logger;
			_hashProvider = hashProvider;
			_baselineStore = baselineStore;
			_inMemoryBaseline = new Dictionary<string, ScanResult>();
		}
		private void LoadBaseline(string path)
		{
			_logger.LogInformation("Loading baseline from file: {Path}", path);
			_inMemoryBaseline.Clear();
			var results = _baselineStore.LoadBaseline(path);
			foreach (var item in results)
			{
				_inMemoryBaseline.Add(item.Path, item);
			}
		}
		private IEnumerable<string> GetPaths()
		{
			if (!_inMemoryBaseline.Any())
			{
				_logger.LogWarning("No baseline loaded. Please load a baseline before checking against it.");
				throw new InvalidOperationException("No baseline loaded. Please load a baseline before checking against it.");
			}
			foreach (var item in _inMemoryBaseline)
			{
				yield return item.Key;
			}
		}
		//No need to check if file exists in baseline, because the paths are loaded from the baseline.
		private IEnumerable<MonitoringAlert> Compare(string path)
		{
			//reminder to add more detailed information for alert message such as file sizes 
			FileInfo file = new(path);
			if (file.Exists)
			{
				using var stream = file.OpenRead();
				var baseline = _inMemoryBaseline[path];

				var hash = _hashProvider.ComputeHash(stream);
				var baselineHash = baseline.Hash;

				var fileSize = file.Length;
				var baselineFileSize = baseline.FileSize;

				if (hash != baselineHash)
				{
					yield return new MonitoringAlert
					(
						path,
						AlertType.HashChanged,
						$"Hash changed for file: {path}.",
						DateTime.UtcNow
					);
				}
				if (fileSize != baselineFileSize)
				{
					yield return new MonitoringAlert
					(
						path,
						AlertType.FileSizeChanged,
						$"File size changed for file: {path}.",
						DateTime.UtcNow
					);
				}
			}
			else
			{
				_logger.LogWarning("File not found: {Path}. It may have been deleted or moved.", path);
				yield return new MonitoringAlert
				(
					path,
					AlertType.Deleted,
					$"File not found: {path}. It may have been deleted or moved.",
					DateTime.UtcNow
				);
			}
		}
		public IEnumerable<MonitoringAlert> Scan(string path)
		{
			LoadBaseline(path);
			var paths = GetPaths();
			if(!paths.Any())
			{
				_logger.LogWarning("No paths found in baseline. Please ensure the baseline is not empty.");
				throw new InvalidOperationException("No paths found in baseline. Please ensure the baseline is not empty.");
			}
			_logger.LogInformation("Scanning files against baseline from file: {Path}", path);
			//No logs for individual files to prevent flooding the log with entries.
			foreach (var item in paths)
			{
				foreach (var alert in Compare(item))
				{
					yield return alert;
				}
			}
		}
		//Reminder to change Start to Event driven polling monitor so alerts are generated in real time and sent to presentation.
		//Reminder to increase the intervalSeconds to a more reasonable value for release.
		public void Start(string path, CancellationToken cancellationToken, int intervalSeconds = 5)
		{
			//LoadBaseline(path);
			//note to add "continuous monitoring" phrase to the log message during Start() implementation.
			throw new NotImplementedException();
		}

		public void Stop()
		{
			throw new NotImplementedException();
		}
	}
}
