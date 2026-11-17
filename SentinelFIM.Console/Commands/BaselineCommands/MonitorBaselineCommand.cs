using Microsoft.Extensions.Logging;
using SentinelFIM.Console.Commands.BaselineCommands.Settings;
using SentinelFIM.Core.Monitoring.Baseline;
using Spectre.Console;
using Spectre.Console.Cli;

namespace SentinelFIM.Console.Commands.BaselineCommands
{
	public class MonitorBaselineCommand : Command<MonitorBaselineSettings>
	{
		private readonly IBaselineFileMonitor _baselineFileMonitor;
		private readonly ILogger<MonitorBaselineCommand> _logger;
		public MonitorBaselineCommand(IBaselineFileMonitor baselineFileMonitor, ILogger<MonitorBaselineCommand> logger)
		{
			_baselineFileMonitor = baselineFileMonitor;
			_logger = logger;
		}

		public void CheckOnce(string baselinePath, int interval, bool once)
		{
			_logger.LogInformation("Checking baseline file {BaselinePath} for changes", baselinePath);
			var results = _baselineFileMonitor.Scan(baselinePath);
			foreach (var result in results)
			{
				AnsiConsole.MarkupLine($"[bold red]Alert:[/] {result.AlertType} - {result.Path}");
			}
			if (results == null)
			{
				_logger.LogInformation("No changes detected in baseline file {BaselinePath}", baselinePath);

				return;
			}
		}
		public override ValidationResult Validate(CommandContext context, MonitorBaselineSettings settings)
		{
			//Added to prevent showing help multiple times
			bool helpShown = false;
			var help = "[yellow]Use -h or --help for more options[/]";

			if (settings.Interval <= 0)
			{
				if(!helpShown) AnsiConsole.MarkupLine(help);
				helpShown = true;
				return ValidationResult.Error("Interval must be greater than 0");
			}
			return ValidationResult.Success();
		}
		public override int Execute(CommandContext context, MonitorBaselineSettings settings, CancellationToken cancellationToken)
		{	
			if (settings.Once) CheckOnce(settings.Baseline, settings.Interval, settings.Once);
			//Reminder to implement continuous monitoring of baseline file changes.
			return 0;
		}
	}
}
