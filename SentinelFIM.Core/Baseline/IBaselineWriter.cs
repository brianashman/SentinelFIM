using SentinelFIM.Core.Models.Baseline;
using System;
using System.Collections.Generic;
using System.Text;

namespace SentinelFIM.Core.Baseline
{
	public interface IBaselineWriter
	{
		string Format { get; }
		BaselineResults SaveBaseline(string path, string outputPath, bool isRecursive, CancellationToken cancellationToken);
	}
}
