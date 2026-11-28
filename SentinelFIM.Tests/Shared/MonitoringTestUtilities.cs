using SentinelFIM.Tests.Models;

namespace SentinelFIM.Tests.Shared
{
	//Reminder to add the cts timeout for tests
	public static class MonitoringTestUtilities
	{
		//Fixture is set in SentinelFIM.Tests.csproj to copy the Test Environment directory relative to the executable path
		public static string GetTestEnvironment()
		{
			var currentDirectory = Directory.GetCurrentDirectory();
			if (string.IsNullOrEmpty(currentDirectory)) throw new InvalidOperationException("Unable to determine the current directory for the test environment.");
			
			const string testEnvironmentDirectoryName = "Test Environment";
			var testEnvironmentDirectory = Directory.GetDirectories(currentDirectory)
				.FirstOrDefault(dir => Path.GetFileName(dir) == testEnvironmentDirectoryName);
			if (string.IsNullOrEmpty(testEnvironmentDirectory)) throw new InvalidOperationException("The test environment is not set to the correct directory.");

			return testEnvironmentDirectory;
		}
		public static void CopyTestEnvironmentToTemp(string sourceDirectory, string tempPath)
		{
			if(!Directory.Exists(sourceDirectory)) throw new InvalidOperationException("The source directory for the test environment does not exist.");
			
			IEnumerable<string> GetDirectories() => Directory.GetDirectories(sourceDirectory, "*", SearchOption.AllDirectories);
			IEnumerable<string> GetFiles() => Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);

			void CopyContents()
			{
				try
				{
					foreach (var directory in GetDirectories())
					{
						var relativePath = Path.GetRelativePath(sourceDirectory, directory);
						var destinationDirectory = Path.Combine(tempPath, relativePath);
						Directory.CreateDirectory(destinationDirectory);
					}
					foreach (var file in GetFiles())
					{
						var relativePath = Path.GetRelativePath(sourceDirectory, file);
						var destinationFile = Path.Combine(tempPath, relativePath);
						File.Copy(file, destinationFile);
					}
				}
				catch (Exception ex)
				{
					throw new InvalidOperationException("Failed to copy the test environment to the temporary directory.", ex);
				}
			}
			CopyContents();
		}
		// Not meant to be truly random, but to simulate changes in the test environment for testing purposes.
		public static HashSet<string> ModifyTestEnvironment(string testEnvironment, SearchOption searchOption, int fileCount = 3)
		{
			var random = new Random();
			string[] testFiles = Directory.GetFiles(testEnvironment, "*", searchOption).Where(file => !Path.GetFileName(file).Contains("baseline", StringComparison.OrdinalIgnoreCase)).ToArray();
			int testFileCount = testFiles.Length;
			if (testFileCount <= 0) throw new InvalidOperationException("No files available for modification in the test environment.");
			

			int modificationCount = random.Next(1, fileCount + 1); // Randomly choose how many files to modify (between 1 and fileCount)
			if(modificationCount > testFileCount) modificationCount = testFileCount; // Ensure more files are not modified than exist

			HashSet<string> filesToModify = new HashSet<string>();
			
			for (int i = 0; i < modificationCount; i++)
			{
				int fileIndex = random.Next(testFileCount);
				if (filesToModify.Add(testFiles[fileIndex]))
				{
					File.AppendAllText(testFiles[fileIndex], $"// Modified at {DateTime.UtcNow}");
				}
				else i--;
			}
			return filesToModify;
		}
		public static string GetCurrentDirectoryBaselinePath(string directory)
		{
			if (!Directory.Exists(directory)) throw new InvalidOperationException("The specified directory does not exist.");

			var files = Directory.GetFiles(directory, "*", SearchOption.TopDirectoryOnly);	

			var result = files.FirstOrDefault(path => Path.GetFileName(path).Contains("baseline", StringComparison.OrdinalIgnoreCase) 
			&& Path.GetFileName(path).StartsWith("current", StringComparison.OrdinalIgnoreCase));
			return result ?? throw new InvalidOperationException("No non-recursive baseline file found in the specified directory.");
		}
		public static string GetRecursiveDirectoryBaselinePath(string directory)
		{
			if (!Directory.Exists(directory)) throw new InvalidOperationException("The specified directory does not exist.");

			var files = Directory.GetFiles(directory, "*", SearchOption.TopDirectoryOnly);

			var result = files.FirstOrDefault(path => Path.GetFileName(path).Contains("baseline", StringComparison.OrdinalIgnoreCase) 
			&& Path.GetFileName(path).StartsWith("all", StringComparison.OrdinalIgnoreCase));
			return result ?? throw new InvalidOperationException("No recursive baseline file found in the specified directory.");
		}
		public static (string tempEnvPath, HashSet<string> modifiedFiles) CreateTestEnvironment(SearchOption searchOption,
			BaselineCreationOption baselineCreationOption,
			CancellationToken cancellationToken, int fileCount = 3)
		{
			// Target directory for the test environment
			var testDirectoryPath = FileSystemTestUtilities.CreateRandomTestDirectory();

			// Existing Test Environment Paths
			var testEnvironmentPaths = MonitoringTestUtilities.GetTestEnvironment();

			// Copies the test environment to the temporary directory for testing
			MonitoringTestUtilities.CopyTestEnvironmentToTemp(testEnvironmentPaths, testDirectoryPath);

			//Creates both root and recursive baseline file for the test environment to be used for scanning
			switch (baselineCreationOption)
			{
				case BaselineCreationOption.RootOnly:
					BaselineTestUtilities.CreateRootBaselineAtPath(testDirectoryPath, cancellationToken);
					break;
				case BaselineCreationOption.RecursiveOnly:
					BaselineTestUtilities.CreateRecursiveBaselineAtPath(testDirectoryPath, cancellationToken);
					break;
				case BaselineCreationOption.RootAndRecursive:
					BaselineTestUtilities.CreateAllBaselinesAtPath(testDirectoryPath, cancellationToken);
					break;
			}
			


			// Modifies the test environment to simulate changes for testing purposes
			var filesChanged = MonitoringTestUtilities.ModifyTestEnvironment(testDirectoryPath, searchOption, fileCount);

			// Path to the temporary test environment for scanning & the set of modified files for validation
			return (testDirectoryPath, filesChanged);
		}
	}
}
