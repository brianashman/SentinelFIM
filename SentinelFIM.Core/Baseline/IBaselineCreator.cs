using SentinelFIM.Core.Models;

namespace SentinelFIM.Core.Baseline
{
	public interface IBaselineCreator
	{
		IEnumerable<ScanResult> CreateBaseline(string path);
	}
}
