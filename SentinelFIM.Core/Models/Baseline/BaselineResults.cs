using System;
using System.Collections.Generic;
using System.Text;

namespace SentinelFIM.Core.Models.Baseline
{
	public record BaselineResults(int FilesSaved, string OutputPath, TimeSpan Duration, long BaselineFileSize);
}
