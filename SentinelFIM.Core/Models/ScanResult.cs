	namespace SentinelFIM.Core.Models
	{
		public class ScanResult
		{
			public required string FileName { get; set; }
			public required string Path { get; set; }
			public required string Hash { get; set; }
			public required string Algorithm { get; set; }
			public long FileSize { get; set; }
			public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
			//for reference later
			//DateOnly.FromDateTime(DateTime.Now);
		}
	}
