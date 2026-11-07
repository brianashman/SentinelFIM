using Microsoft.Extensions.Logging;
using SentinelFIM.Core.Baseline;
using SentinelFIM.Core.Files;
using SentinelFIM.Core.Identifiers;
using SentinelFIM.Core.Models;
using SentinelFIM.Infrastructure.Baseline.Creator;
using System.Text.Json;

namespace SentinelFIM.Infrastructure.Baseline.Store
{
	// Algorithm is autosupplied via DI container temporarily for baselinecreator.
	// This class is responsible for saving and loading baseline data in JSON format.
	public class JsonBaselineStore : IBaselineStore
	{
		public string Format => OutputFormat.Json;
		private readonly ILogger<JsonBaselineStore> _logger;
		private readonly BaselineCreator _baselineCreator;
		public JsonBaselineStore(ILogger<JsonBaselineStore> logger, BaselineCreator baselineCreator)
		{
			_logger = logger;
			_baselineCreator = baselineCreator;
		}


		public IEnumerable<ScanResult> LoadBaseline(string path)
		{
			FileInfo baseline = new FileInfo(path);

			if(!baseline.Exists)
			{
				_logger.LogWarning("Baseline file not found: {0}", path);
				throw new FileNotFoundException($"Baseline file not found: {path}");
			}
			var serializerOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
			};

			_logger.LogInformation("Loading baseline from file: {0}", path);
			using var stream = baseline.OpenRead();
			var scanResults = JsonSerializer.Deserialize<IEnumerable<ScanResult>>(
				stream, 
				serializerOptions
				);
			return scanResults!;	
		}

		public void SaveBaseline(string path, string outputFile, BaselineScanType scanType)
		{
			IEnumerable<ScanResult> baseline = scanType switch
			{
				BaselineScanType.Directory => _baselineCreator.CreateDirectoryBaseline(path),
				BaselineScanType.File => new [] { _baselineCreator.CreateFileBaseline(path) },
				_ => throw new ArgumentException("Invalid value for scanType parameter", nameof(scanType))
			};
			try
			{
				_logger.LogInformation("Attempting to save baseline to file: {0}", path);
				var serializerOptions = new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true,
					WriteIndented = true
				};
				string json = JsonSerializer.Serialize(baseline, serializerOptions);
				using (StreamWriter writer = new(outputFile))
				{
					writer.Write(json);
				}
			}
			catch (Exception)
			{
				_logger.LogError("Failed to save baseline file: {0}", path);
				throw;
			}
			_logger.LogInformation("Baseline saved successfully to file: {0}", path);
		}
	}
}
