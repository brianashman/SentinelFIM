using SentinelFIM.Console.Commands.Settings;
using SentinelFIM.Core.Baseline;
using Spectre.Console;
using Spectre.Console.Cli;

namespace SentinelFIM.Console.Commands
{
	public class TestCommand : Command<TestSettings>
	{
		public TestCommand()
		{
		}

		public override int Execute(CommandContext context, TestSettings settings, CancellationToken cancellationToken)
		{
			AnsiConsole.MarkupLine($"[green]Test command {settings.Something} executed successfully![/]");
			return 1;
		}
	}
}
