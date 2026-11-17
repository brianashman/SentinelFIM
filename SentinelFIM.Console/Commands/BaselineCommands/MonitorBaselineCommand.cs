using SentinelFIM.Console.Commands.BaselineCommands.Settings;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Text;

namespace SentinelFIM.Console.Commands.BaselineCommands
{
	public class MonitorBaselineCommand : Command<MonitorBaselineSettings>
	{
		public override int Execute(CommandContext context, MonitorBaselineSettings settings, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
