using Spectre.Console.Cli;

namespace SentinelFIM.Console.Commands.Settings
{
	public class TestSettings : CommandSettings
	{
		[CommandArgument(0, "<something>")]
		public string Something { get; set; } = String.Empty;
	}
}
