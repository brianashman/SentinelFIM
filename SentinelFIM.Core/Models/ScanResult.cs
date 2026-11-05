namespace SentinelFIM.Core.Models
{
	public class ScanResult
	{
		public string FileName { get; set; }
		public string Path { get; set; }
		public string Hash { get; set; }
		public string Algorithm { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		//for reference later
		//DateOnly.FromDateTime(DateTime.Now);
	}
}
