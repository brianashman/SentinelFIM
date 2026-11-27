using Microsoft.Extensions.Logging;
using SentinelFIM.Core.Baseline;
using SentinelFIM.Core.Identifiers;
using SentinelFIM.Core.Models;
using SentinelFIM.Core.Models.Baseline;
using SentinelFIM.Infrastructure.Baseline.Creator;
using System.Diagnostics;
using System.Text.Json;

namespace SentinelFIM.Infrastructure.Baseline.Store.Json
{
	// Algorithm is autosupplied via DI container temporarily for baselinecreator.
	// This class is responsible for saving and loading baseline data in JSON format.
	public class JsonBaselineReader : IBaselineReader
	{
		public string Format => OutputFormat.Json;
		
		private readonly ILogger<JsonBaselineReader> _logger;
		public JsonBaselineReader(ILogger<JsonBaselineReader> logger)
		{
			_logger = logger;
		}

		public BaselineDocument LoadBaseline(string path)
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
			var scanResults = JsonSerializer.Deserialize<BaselineDocument>(
				stream, 
				serializerOptions
				);
			return scanResults!;	
		}
	}
}
