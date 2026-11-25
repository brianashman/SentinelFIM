using SentinelFIM.Core.Baseline;
using SentinelFIM.Core.Identifiers;
using SentinelFIM.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SentinelFIM.Infrastructure.Baseline.Store.Json
{
	// Composite interface representing a baseline storage provider.
	// Reader and Writer were separated to reduce unnecessary dependencies
	// for consumers that only require one capability.
	public class JsonBaselineStore : IBaselineStore
	{
		public string Format => OutputFormat.Json;

		private readonly IBaselineReader _baselineReader;
		private readonly IBaselineWriter _baselineWriter;

		public JsonBaselineStore(IBaselineReader baselineReader, IBaselineWriter baselineWriter)
		{
			_baselineReader = baselineReader;
			_baselineWriter = baselineWriter;
		}	

		public IEnumerable<ScanResult> LoadBaseline(string path)
		{
			return _baselineReader.LoadBaseline(path);
		}

		public BaselineResults SaveBaseline(string path, string outputPath, bool isRecursive, CancellationToken cancellationToken)
		{
			return _baselineWriter.SaveBaseline(path, outputPath, isRecursive, cancellationToken);
		}
	}
}
