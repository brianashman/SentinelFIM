using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SentinelFIM.Console.Commands.BaselineCommands.Settings
{
	public class MonitorBaselineSettings : CommandSettings
	{
		[CommandArgument(0, "<Path>")]
		[Description("The path to the baseline file to monitor")]
		public string Baseline { get; init; }
	}
}
