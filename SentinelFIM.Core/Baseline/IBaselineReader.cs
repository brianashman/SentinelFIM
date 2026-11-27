using SentinelFIM.Core.Models;
using SentinelFIM.Core.Models.Baseline;
using System;
using System.Collections.Generic;
using System.Text;

namespace SentinelFIM.Core.Baseline
{
	public interface IBaselineReader
	{
		string Format { get; }
		BaselineDocument LoadBaseline(string path);
	}
}
