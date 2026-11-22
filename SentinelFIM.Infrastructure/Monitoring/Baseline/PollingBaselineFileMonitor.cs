using Microsoft.Extensions.Logging;
using SentinelFIM.Core.Baseline;
using SentinelFIM.Core.Files;
using SentinelFIM.Core.Hashing;
using SentinelFIM.Core.Models;
using SentinelFIM.Core.Models.Monitoring;
using SentinelFIM.Core.Monitoring.Baseline;
using System.Diagnostics;

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

	// TODO:
	// Add monitoring exporter factory.
	// Exporters selected by self-identifying format name.
	// Monitor optionally exports alerts as they are generated.
	// Reminder to expose a property to set interval at runtime for GUI specifically.

	// TODO:
	// Continuous polling generates duplicate alerts for files that remain
	// in the same state across scans. When file logging is implemented,
	// track alert state transitions and only log detected/resolved events.
	public class PollingBaselineFileMonitor : IBaselineFileMonitor
	{
		private readonly ILogger<PollingBaselineFileMonitor> _logger;
		private readonly Dictionary<string, ScanResult> _inMemoryBaseline;
		private readonly IHashProvider _hashProvider;
		private readonly IBaselineStore _baselineStore;
		private string? baselinePath = null;
		public event EventHandler<MonitoringStatus>? StatusChanged;

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
				FileStream stream = null!;
				try
				{
					stream = file.OpenRead();
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error opening file: {Path}", path);
					yield break;
				}
				var baseline = _inMemoryBaseline[path];
				var fileHash = string.Empty;
				using (stream)
				{
					fileHash = _hashProvider.ComputeHash(stream);
				}
				var baselineHash = baseline.Hash;

				var fileSize = file.Length;
				var baselineFileSize = baseline.FileSize;

				var lastModified = file.LastWriteTimeUtc;

				var fileName = file.Name;

				if (fileHash != baselineHash)
				{
					//_logger.LogWarning("Hash changed for file: {Path}. Baseline hash: {BaselineHash}, Current hash: {CurrentHash}", path, baselineHash, fileHash);
					yield return new MonitoringAlert
					(
						path,
						AlertType.HashChanged,
						$"Hash changed for file: {fileName}.",
						DateTime.UtcNow,
						lastModified,
						baselineHash,
						fileHash
					);
				}
				if (fileSize != baselineFileSize)
				{
					//_logger.LogWarning("File size changed for file: {Path}. Baseline size: {BaselineSize}, Current size: {CurrentSize}", path, baselineFileSize, fileSize);
					yield return new MonitoringAlert
					(
						path,
						AlertType.FileSizeChanged,
						$"File size changed for file: {fileName}.",
						DateTime.UtcNow,
						lastModified,
						baselineFileSize.ToString(),
						fileSize.ToString()
					);
				}
			}
			else
			{
				//Reminder to add more detailed information for alert message
				//_logger.LogWarning("File not found: {Path}. It may have been deleted or moved.", path);
				yield return new MonitoringAlert
				(
					path,
					AlertType.Deleted,
					//Cannot use file.Name because file does not exist, so we cannot get the name from the FileInfo object.
					$"File not found: {path}. It may have been deleted or moved.",
					DateTime.UtcNow,
					null,
					null,
					null
				);
			}
		}
		public IEnumerable<MonitoringAlert> Scan(string path, CancellationToken cancellationToken)
		{
			//Load the Baseline if it has not been loaded yet.
			if (baselinePath != path)
			{
				baselinePath = path;
				LoadBaseline(path);
			}

			var paths = GetPaths();
			_logger.LogInformation("Scanning files against baseline from file: {Path}", path);
			//No logs for individual files to prevent flooding the log with entries.
			foreach (var item in paths)
			{
				cancellationToken.ThrowIfCancellationRequested();
				foreach (var alert in Compare(item))
				{
					yield return alert;
				}
			}
		}
		public IEnumerable<MonitoringAlert> Monitor(string path, int intervalSeconds, CancellationToken cancellationToken)
		{
			StatusChanged?.Invoke(this, new MonitoringStatus(
					MonitoringStatusType.Started,
					$"Continuous monitoring in progress for baseline file: {path}",
					DateTime.UtcNow
				));
			_logger.LogInformation("Continuous monitoring started for baseline file: {Path}", path);
			var monitoringTimer = Stopwatch.StartNew();
			try
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					var intervalTimer = Stopwatch.StartNew();
					var alerts = Scan(path, cancellationToken);

					foreach (var alert in alerts)
					{
						yield return alert;
					}
					intervalTimer.Stop();
					var timeRemaining = TimeSpan.FromSeconds(intervalSeconds) - intervalTimer.Elapsed;
					if (timeRemaining < TimeSpan.Zero)
					{
						StatusChanged?.Invoke(this, new MonitoringStatus(
							MonitoringStatusType.IntervalExceeded, 
							$"Scan completed in {intervalTimer.Elapsed.TotalSeconds} seconds, which is longer than the specified interval of {intervalSeconds} seconds. Consider increasing the interval.", 
							DateTime.UtcNow));
						_logger.LogWarning("Scan completed in {ElapsedSeconds} seconds, which is longer than the specified interval of {IntervalSeconds} seconds.", intervalTimer.Elapsed.TotalSeconds, intervalSeconds);
						timeRemaining = TimeSpan.Zero;
					}

					if (cancellationToken.WaitHandle.WaitOne(timeRemaining))
					{
						yield break; //Stop monitoring if cancellation is requested
					}
				}
			}
			finally
			{
				monitoringTimer.Stop();
				StatusChanged?.Invoke(this, new MonitoringStatus(
					MonitoringStatusType.Stopped,
					$"Continuous monitoring stopped - Cancellation requested.\nTotal time: {monitoringTimer.Elapsed.TotalSeconds} seconds",
					DateTime.UtcNow));
				_logger.LogInformation("Continuous monitoring stopped for baseline file: {Path}", path);
			}
		}
	}
}
