using SentinelFIM.Console.Commands;
using Spectre.Console;

namespace SentinelFIM.Console.Controller
{
	public class CLIController
	{
		private readonly CreateBaselineCommand _createBaselineCommand;

		public CLIController
			(CreateBaselineCommand createBaselineCommand)
		{
			_createBaselineCommand = createBaselineCommand;
		}

		public void Execute()
		{

		}

	}
}
