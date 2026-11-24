using System;
using System.Collections.Generic;
using System.Text;

namespace SentinelFIM.Tests.Shared
{
	public static class FileSystemTestUtilities
	{
		public static string CreateRandomTestDirectory()
		{
			var directoryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			Directory.CreateDirectory(directoryPath);

			return directoryPath;
		}
		public static string CreateSubDirectory(string parentDirectory, string subdirectoryName)
		{
			var subdirectoryPath = Path.Combine(parentDirectory, subdirectoryName);
			Directory.CreateDirectory(subdirectoryPath);
			return subdirectoryPath;
		}
		public static string CreateRandomTestDirectoryWithFiles(params string[] fileName)
		{
			var directoryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			Directory.CreateDirectory(directoryPath);

			foreach (var file in fileName)
			{
				var filePath = Path.Combine(directoryPath, file);
				File.WriteAllText(filePath, contents: file); //"contents:" explicitly added for readability
			}
			return directoryPath;
		}
		public static void AddFilesToDirectory(string directoryPath, params string[] fileNames)
		{
			if (!Directory.Exists(directoryPath)) throw new DirectoryNotFoundException($"The specified directory '{directoryPath}' does not exist.");
			foreach (var file in fileNames)
			{
				var filePath = Path.Combine(directoryPath, file);
				File.WriteAllText(filePath, contents: file); //"contents:" explicitly added for readability
			}
		}
		public static void DeleteTestDirectoryRecursive(string directoryPath)
		{
			if (Directory.Exists(directoryPath)) Directory.Delete(directoryPath, true);
		}
	}
}
