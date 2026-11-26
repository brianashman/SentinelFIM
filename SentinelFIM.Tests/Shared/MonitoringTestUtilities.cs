using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Internal;

namespace SentinelFIM.Tests.Shared
{
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
		public static string CopyTestEnvironmentToTemp(string sourceDirectory, string tempPath)
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
			return tempPath;
		}
	}
}
