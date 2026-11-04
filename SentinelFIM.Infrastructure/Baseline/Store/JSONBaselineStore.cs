using SentinelFIM.Core.Baseline;
using SentinelFIM.Core.Identifiers;
using SentinelFIM.Core.Models;

namespace SentinelFIM.Infrastructure.Baseline.Store
{
	public class JsonBaselineStore : IBaselineStore
	{
		public string Format => OutputFormat.Json;

		public IEnumerable<ScanResult> LoadBaseline(string path)
		{
			throw new NotImplementedException();
		}

		public void SaveBaseline(string path, IEnumerable<ScanResult> scanResults)
		{
			throw new NotImplementedException();
		}
	}
}
