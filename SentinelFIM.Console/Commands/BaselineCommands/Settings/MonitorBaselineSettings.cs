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

		//Reminder to change interval later
		//Intentionally using string to avoid parsing issues with Spectre.Console.Cli
		[CommandOption("-i|--interval <Interval>")]
		[Description("The interval at which to check the baseline")]
		[DefaultValue("15")]
		public string Interval { get; init; }

		[CommandOption("-o|--once")]
		[Description("Only check the baseline once, instead of continuously")]
		public bool Once { get; init; }
	}
}
