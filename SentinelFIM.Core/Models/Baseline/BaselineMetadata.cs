using System;
using System.Collections.Generic;
using System.Text;

namespace SentinelFIM.Core.Models.Baseline
{
	public record BaselineMetadata (string OutputFormat, DateTime CreatedAt, bool IsRecursive);
}
