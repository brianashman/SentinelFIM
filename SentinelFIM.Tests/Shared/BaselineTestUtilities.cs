using Microsoft.Extensions.Logging;
using Moq;
using SentinelFIM.Infrastructure.Baseline.Creator;
using SentinelFIM.Infrastructure.Baseline.Store.Json;
using SentinelFIM.Infrastructure.Files;
using SentinelFIM.Infrastructure.Hashing;

namespace SentinelFIM.Tests.Shared
{
	/// <summary> Provides shared baseline test dependencies. </summary>
	/// <remarks>
	/// Provides helper methods for generating test baselines.
	/// Uses JSON serialization and SHA256 hashing by default.
	/// </remarks>
	public static class BaselineTestUtilities
	{
		// Logger Mocks for testing
		private static readonly Mock<ILogger<JsonBaselineWriter>> _loggerWriterMock = new Mock<ILogger<JsonBaselineWriter>>();
		private static readonly Mock<ILogger<BaselineCreator>> _loggerCreatorMock = new Mock<ILogger<BaselineCreator>>();
		private static readonly Mock<ILogger<FileSystemProvider>> _loggerFSMock = new Mock<ILogger<FileSystemProvider>>();

		//Composition of dependencies for testing purposes
		private static readonly SHA256HashProvider _hashProvider = new SHA256HashProvider();
		private static readonly FileSystemProvider _fileSystemProvider = new FileSystemProvider(_loggerFSMock.Object);
		private static readonly BaselineCreator _baselineCreator = new BaselineCreator(_loggerCreatorMock.Object, _fileSystemProvider, _hashProvider);
		private static readonly JsonBaselineWriter _baselineWriter = new JsonBaselineWriter(_loggerWriterMock.Object, _baselineCreator);

		//Methods create baseline using Path.Combine to ensure the path is not set to exe directory
		//but rather the test fixture copy.

		/// <summary>
		/// Creates a Non-Recursive baseline as the specified path
		/// </summary>
		/// <param name="path"></param>
		public static void CreateRootBaselineAtPath(string path, CancellationToken ct)
		{
			string outputPath = Path.Combine(path, "currentfolder_baseline.json");
			_baselineWriter.SaveBaseline(path, outputPath: outputPath, isRecursive: false, ct);
		}

		/// <summary>
		/// Creates a recursive baseline at the specified path.
		/// </summary>
		/// <param name="path"></param>
		public static void CreateRecursiveBaselineAtPath(string path, CancellationToken ct)
		{
			string outputPath = Path.Combine(path, "allfolder_baseline.json");
			_baselineWriter.SaveBaseline(path, outputPath: outputPath, isRecursive: true, ct);
		}

		/// <summary>
		/// Creates both root and recursive baselines at the specified path.
		/// </summary>
		/// <param name="path"></param>
		public static void CreateAllBaselinesAtPath(string path, CancellationToken ct)
		{
			CreateRootBaselineAtPath(path, ct);
			CreateRecursiveBaselineAtPath(path, ct);
		}

	}
}
