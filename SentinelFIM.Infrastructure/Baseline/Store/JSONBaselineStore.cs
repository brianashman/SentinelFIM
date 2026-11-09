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
	// Note: Add a choice to choose a directory to save the file to if the baseline is created. (Baseline Saves to current directory only temporarily)
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

		public (int filesSaved, string outputPath) SaveBaseline(string path, string outputFile, BaselineScanType scanType)
		{
			var currentDirectory = Directory.GetCurrentDirectory();
			List<ScanResult> baseline = scanType switch
			{
				BaselineScanType.Directory => [.. _baselineCreator.CreateDirectoryBaseline(path)],
				BaselineScanType.File => [_baselineCreator.CreateFileBaseline(path)],
				_ => throw new ArgumentException("Invalid value for scanType parameter", nameof(scanType))
			};
			try
			{
				_logger.LogInformation("Attempting to save baseline to path: {OutputPath}", Path.Combine(currentDirectory, outputFile));
				var serializerOptions = new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true,
					WriteIndented = true
				};
				FileStream fileStream = new(Path.Combine(currentDirectory, outputFile), FileMode.Create, FileAccess.Write);
				JsonSerializer.Serialize(fileStream, baseline, serializerOptions);
			}
			catch (Exception)
			{
				_logger.LogError("Failed to save baseline file: {Path}", Path.Combine(currentDirectory, outputFile));
				throw;
			}
			if (File.Exists(outputFile))
			{
				_logger.LogInformation("Baseline saved successfully to file: {OutputPath}", Path.Combine(currentDirectory, outputFile));
			}
			else
			{
				_logger.LogError("Failed to save baseline file: {OutputPath}", Path.Combine(currentDirectory, outputFile));
				throw new IOException($"Failed to save baseline file: {Path.Combine(currentDirectory, outputFile)}");
			}
			return (baseline.Count, Path.Combine(currentDirectory, outputFile));
		}
	}
}
