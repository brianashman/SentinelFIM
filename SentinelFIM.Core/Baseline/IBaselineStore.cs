using SentinelFIM.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SentinelFIM.Core.Baseline
{
	public interface IBaselineStore
	{
		string Format { get; }
		IEnumerable<ScanResult> LoadBaseline(string path);
		void SaveBaseline(string path, string outputFile, BaselineScanType scanType);
	}
}
