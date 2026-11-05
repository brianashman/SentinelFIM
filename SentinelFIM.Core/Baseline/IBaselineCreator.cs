using SentinelFIM.Core.Models;

namespace SentinelFIM.Core.Baseline
{
	public interface IBaselineCreator
	{
		IEnumerable<ScanResult> CreateDirectoryBaseline(string directoryPath, string algorithm);
		ScanResult CreateFileBaseline(string path, string algorithm);
	}
}
