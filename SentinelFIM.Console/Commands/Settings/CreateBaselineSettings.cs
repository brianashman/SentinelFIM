using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SentinelFIM.Console.Commands.Settings
{
	public class CreateBaselineSettings: CommandSettings
	{
		private const string DefaultPath = ".";
		private const string DefaultAlgorithm = "sha256";

		[CommandArgument(0, "[Path]")]
		[Description("The path to create the baseline for")]
		[DefaultValue(DefaultPath)]
		public string Path { get; init; }


		[CommandOption("-p|--preview")]
		[Description("Displays a summary of the baseline without saving it")]
		public bool Preview { get; init; }

		/// <summary>
		/// Reminder to add factory logic for runtime injection
		/// Removed because only one IHashProvider
		/// </summary>
		//[CommandOption("-a|--algorithm <algorithm>")]
		//[Description("The algorithm to use for creating the baseline")]
		//[DefaultValue(DefaultAlgorithm)]
		//public string Algorithm { get; init; }

		[CommandOption("-o|--output <outputFile>")]
		[Description("The file to save the baseline to")]
		public string OutputFile { get; init; }
		
		/// <summary>
		/// Reminder to add factory logic
		/// Removed because no factory to pass this value into
		/// </summary>
		//[CommandOption("-t|--type <outputType>")]
		//[Description("The output format")]
		//[DefaultValue("json")]
		//public string OutputType { get; init; }

	}
}
