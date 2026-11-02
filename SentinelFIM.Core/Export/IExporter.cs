namespace SentinelFIM.Core.Export
{
	public interface IExporter
	{
		void Export(SentinelFIM.Core.Models.ScanResult result);
	}
}
