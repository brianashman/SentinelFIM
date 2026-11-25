using Microsoft.Extensions.Logging;
using SentinelFIM.Core.Baseline;
using SentinelFIM.Core.Identifiers;
using SentinelFIM.Core.Models;
using SentinelFIM.Infrastructure.Baseline.Creator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace SentinelFIM.Infrastructure.Baseline.Store.Json
{
	public class JsonBaselineWriter : IBaselineWriter
	{
		public string Format => OutputFormat.Json;
		private readonly ILogger<JsonBaselineReader> _logger;
		private readonly BaselineCreator _baselineCreator;
		public JsonBaselineWriter(ILogger<JsonBaselineReader> logger, BaselineCreator baselineCreator)
		{
			_logger = logger;
			_baselineCreator = baselineCreator;
		}
		public BaselineResults SaveBaseline(string path, string outputPath, bool isRecursive, CancellationToken cancellationToken)
		{
			Stopwatch timer = new();
			string createdOutputFile = Path.GetFullPath(outputPath);

			bool isDirectory = Directory.Exists(path);
			bool isFile = File.Exists(path);

			timer.Start();
			List<ScanResult> baseline = new();
			if (isDirectory)
			{
				baseline.AddRange(_baselineCreator.CreateDirectoryBaseline(path, isRecursive, cancellationToken));
			}
			else if (isFile)
			{
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
