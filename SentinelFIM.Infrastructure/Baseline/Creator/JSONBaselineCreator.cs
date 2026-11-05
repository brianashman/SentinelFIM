using Microsoft.Extensions.Logging;
using SentinelFIM.Core.Baseline;
using SentinelFIM.Core.Files;
using SentinelFIM.Core.Hashing;
using SentinelFIM.Core.Models;

namespace SentinelFIM.Infrastructure.Baseline.Creator
{
	public class JSONBaselineCreator : IBaselineCreator
	{
		private readonly ILogger<JSONBaselineCreator> _logger;
		private readonly IFileProvider _fileProvider;
		private readonly IHashProvider _hashProvider;
		public JSONBaselineCreator(ILogger<JSONBaselineCreator> logger, IFileProvider fileProvider, IHashProvider hashProvider)
		{
			_logger = logger;
			_fileProvider = fileProvider;
			_hashProvider = hashProvider;
		}
		public IEnumerable<ScanResult> CreateDirectoryBaseline(string directoryPath)
		{
			DirectoryInfo info = new(directoryPath);
			// Validate the directory path
			if(!info.Exists) throw new DirectoryNotFoundException("Directory not found: " + directoryPath);

			
			IEnumerable<string> files = _fileProvider.EnumeratePaths(directoryPath, EnumerationSearchOption.AllDirectories);
			foreach (var file in files)
			{
				yield return CreateFileBaseline(file);
			}
		}

		public ScanResult CreateFileBaseline(string path)
		{
			FileInfo info = new(path);

			if(!info.Exists) throw new FileNotFoundException("File not found: " + path);

			using Stream stream = File.OpenRead(path);

			return new ScanResult
			{
				FileName = info.Name,
				Path = path,
				Algorithm = _hashProvider.Algorithm,
				Hash = _hashProvider.ComputeHash(stream)
			};
		}
	}
}
