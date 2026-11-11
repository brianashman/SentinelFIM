using System;
using System.Collections.Generic;
using System.Text;

namespace SentinelFIM.Core.Models
{
	public record BaselineResults(int FilesSaved, string OutputPath, TimeSpan Duration, long TotalFileSize);
}
