using Microsoft.Extensions.Logging;
using SentinelFIM.Core.Baseline;
using SentinelFIM.Core.Files;
using SentinelFIM.Core.Identifiers;
using SentinelFIM.Core.Models;
using SentinelFIM.Infrastructure.Baseline.Creator;
using System.Diagnostics;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography.X509Certificates;
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
				_logger.LogWarning("Baseline file not found: {Path}", path);
				throw new FileNotFoundException($"Baseline file not found: {path}");
			}
			var serializerOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
			};

			_logger.LogInformation("Loading baseline from file: {Path}", path);
			using var stream = baseline.OpenRead();
			var scanResults = JsonSerializer.Deserialize<IEnumerable<ScanResult>>(
				stream, 
				serializerOptions
				);
			return scanResults!;	
		}

		public BaselineResults SaveBaseline(string path, string outputPath)
		{
			Stopwatch timer = new();
			string createdOutputFile = Path.GetFullPath(outputPath);

			bool isDirectory = Directory.Exists(path);
			bool isFile = File.Exists(path);

			timer.Start();
			List<ScanResult> baseline = new();
			if (isDirectory)
			{
				_logger.LogInformation("Creating baseline for directory: {Path}", path);
				baseline.AddRange(_baselineCreator.CreateDirectoryBaseline(path));
			}
			else if (isFile)
			{
				_logger.LogInformation("Creating baseline for file: {Path}", path);
				baseline.Add(_baselineCreator.CreateFileBaseline(path));
			}
			else
			{
				timer.Stop();
				_logger.LogError("Path does not exist: {Path}", path);
				throw new FileNotFoundException($"Path does not exist: {path}");
			}
			try
			{
				_logger.LogInformation("Attempting to save baseline to path: {OutputPath}", createdOutputFile);
				if (baseline.Count == 0)
				{
					_logger.LogWarning("No baseline data to save.");
					return new BaselineResults(0, String.Empty, TimeSpan.Zero, 0);
				}
				var serializerOptions = new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true,
					WriteIndented = true
				};
				using FileStream fileStream = new(createdOutputFile, FileMode.Create, FileAccess.Write);
				JsonSerializer.Serialize(fileStream, baseline, serializerOptions);
				timer.Stop();
			}
			catch (Exception)
			{
				_logger.LogError("Failed to save baseline file: {Path}", createdOutputFile);
				timer.Stop();
				throw;
			}
			long length = new FileInfo(createdOutputFile).Length;
			return new BaselineResults(baseline.Count, createdOutputFile, timer.Elapsed, length);
		}
	}
}
