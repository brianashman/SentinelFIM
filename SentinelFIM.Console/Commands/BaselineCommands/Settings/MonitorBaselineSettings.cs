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
		public required string Baseline { get; init; }

		//Reminder to change interval later
		[CommandOption("-i|--interval <Interval>")]
		[Description("The interval at which to check the baseline")]
		[DefaultValue(15)]
		public int Interval { get; init; }

		[CommandOption("-o|--once")]
		[Description("Only check the baseline once, instead of continuously")]
		public bool Once { get; init; }
	}
}
