using SentinelFIM.Core.Models;

namespace SentinelFIM.Core.Export
{
	public interface IExporter
	{
		void Export(ScanResult result);
	}
}
