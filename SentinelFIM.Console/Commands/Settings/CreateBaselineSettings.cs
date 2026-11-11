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


		[CommandOption("-v|--preview")]
		[Description("Displays a summary of the baseline without saving it")]
		public bool Preview { get; init; }

		[CommandOption("-a|--algorithm <algorithm>")]
		[Description("The algorithm to use for creating the baseline")]
		[DefaultValue(DefaultAlgorithm)]
		public string Algorithm { get; init; }

		[CommandOption("-o|--output <outputFile>")]
		[Description("The file to save the baseline to")]
		public string OutputFile { get; init; }
	}
}
