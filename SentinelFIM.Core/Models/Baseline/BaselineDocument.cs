using System;
using System.Collections.Generic;
using System.Text;

namespace SentinelFIM.Core.Models.Baseline
{
	public record BaselineDocument (BaselineMetadata Metadata, IReadOnlyCollection<ScanResult> Results);
}
