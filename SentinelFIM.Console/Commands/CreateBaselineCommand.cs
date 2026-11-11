using SentinelFIM.Console.Commands.Settings;
using SentinelFIM.Core.Baseline;
using SentinelFIM.Core.Models;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Text;

namespace SentinelFIM.Console.Commands
{
	public class CreateBaselineCommand: Command<CreateBaselineSettings>
	{
		private readonly IBaselineStore _baselineStore;
		public CreateBaselineCommand(IBaselineStore baselineStore)
		{
			_baselineStore = baselineStore;
		}
		public void Execute(string path, string outputFile)
		{

			_baselineStore.SaveBaseline(path, outputFile);
		}

		public override int Execute(CommandContext context, CreateBaselineSettings settings, CancellationToken cancellationToken)
		{
			_baselineStore.SaveBaseline(settings.Path, "baseline.json");

			return 0;
		}
	}
}
