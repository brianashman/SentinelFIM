using Microsoft.Extensions.Logging;
using SentinelFIM.Console.Commands.BaselineCommands.Settings;
using SentinelFIM.Core.Models.Monitoring;
using SentinelFIM.Core.Monitoring.Baseline;
using Spectre.Console;
using Spectre.Console.Cli;

namespace SentinelFIM.Console.Commands.BaselineCommands
{
	public class MonitorBaselineCommand : Command<MonitorBaselineSettings>
	{
		private readonly IIntegrityMonitor _baselineFileMonitor;
		private readonly ILogger<MonitorBaselineCommand> _logger;

		// Starts at 9 so the first interval warning is shown immediately.
		// Afterwards, the warning is only shown every 10 occurrences.
		private int counter = 9;
		public MonitorBaselineCommand(IIntegrityMonitor baselineFileMonitor, 
			ILogger<MonitorBaselineCommand> logger)
		{
			_baselineFileMonitor = baselineFileMonitor;
			_logger = logger;
		}

		private void CheckOnce(string baselinePath, int interval, bool once)
		{
			_logger.LogInformation("Checking baseline file {BaselinePath} for changes", baselinePath);
			var results = _baselineFileMonitor.Scan(baselinePath, CancellationToken.None);

			
			foreach (var result in results)
			{
				AnsiConsole.MarkupLine($"[bold red]Alert:[/] {result.AlertType}");
				AnsiConsole.MarkupLine($"[bold yellow]Message:[/] {result.AlertMessage}");
				AnsiConsole.MarkupLine($"[bold yellow]Path:[/] {result.Path}");
				if (result.LastModified != null) AnsiConsole.MarkupLine($"[bold yellow]Last Modified:[/] {result.LastModified?.ToLocalTime().ToString()}");
				else AnsiConsole.MarkupLine($"[bold yellow]Last Modified:[/] N/A");
				if(result.AlertType is AlertType.FileSizeChanged && result.PreviousValue != null && result.CurrentValue != null)
				{
					AnsiConsole.MarkupLine($"[bold yellow]Previous Value:[/] {result.PreviousValue} bytes");
					AnsiConsole.MarkupLine($"[bold yellow]Current Value:[/] {result.CurrentValue} bytes");
				}
				else
				{
					if(result.PreviousValue != null && result.CurrentValue != null)
					{
						AnsiConsole.MarkupLine($"[bold yellow]Previous Value:[/] {result.PreviousValue}");
						AnsiConsole.MarkupLine($"[bold yellow]Current Value:[/] {result.CurrentValue}");
					}
					else
					{
						AnsiConsole.MarkupLine($"[bold yellow] Previous Value:[/] N/A");
						AnsiConsole.MarkupLine($"[bold yellow] Current Value:[/] N/A");
					}
				}
				AnsiConsole.MarkupLine($"[bold yellow]Timestamp:[/] {result.Timestamp.ToLocalTime().ToString()}");

				var rule = new Rule($"[bold red]End of Alert[/]").RuleStyle("red").DottedBorder();
				AnsiConsole.Write(rule);
				AnsiConsole.WriteLine();
			}

			if (results == null)
			{
				_logger.LogInformation("No changes detected in baseline file {BaselinePath}", baselinePath);

				return;
			}
		}
		private void OnStatusChanged(object? sender, MonitoringStatus status)
		{

			if (status.Status == MonitoringStatusType.Started)
			{
				AnsiConsole.MarkupLine($"[green]Monitoring started at {status.TimeStamp.ToLocalTime()}[/]");
			}
			else if (status.Status == MonitoringStatusType.Stopped)
			{
				AnsiConsole.MarkupLine($"[red]Monitoring stopped at {status.TimeStamp.ToLocalTime()}[/]");
			}
			else if (status.Status == MonitoringStatusType.Error)
			{
				AnsiConsole.MarkupLine($"[red]Monitoring error at {status.TimeStamp.ToLocalTime()}[/]");
			}
			else if (status.Status == MonitoringStatusType.IntervalExceeded)
			{	
				counter+=1;
				if (counter == 10)
				{
					AnsiConsole.MarkupLine($"[yellow]Monitoring interval exceeded at {status.TimeStamp.ToLocalTime()}[/]");
					AnsiConsole.MarkupLine($"[yellow]{status.Message}[/]");
					counter = 0;
				}
				
			}
		}
		//Reminder to implement a way to change the monitoring implementation to prevent polling only.
		private void CheckContinuously(string baselinePath, int? interval, bool once, CancellationToken cancellationToken)
		{
			var baselineIterator = _baselineFileMonitor.Monitor(baselinePath, interval, cancellationToken);

			AnsiConsole.MarkupLine("[purple]Note: Only polling approach is currently implemented[/]");

			AnsiConsole.WriteLine();

			AnsiConsole.MarkupLine($"[yellow]Monitoring baseline file {baselinePath} using interval: {interval} seconds.[/]");
			
			AnsiConsole.WriteLine();
			var panel = new Panel("[yellow]Press [bold red]Ctrl+C[/] to stop monitoring[/]").BorderColor(Color.Red);
			AnsiConsole.Write(panel);


			AnsiConsole.WriteLine();

			_baselineFileMonitor.StatusChanged += OnStatusChanged;

			
			try
			{
				foreach (var alert in baselineIterator)
				{
					
					var rule = new Rule($"[bold red]Alert Detected[/]").RuleStyle("red").DottedBorder();
					AnsiConsole.Write(rule);

					AnsiConsole.MarkupLine($"[blue]{alert.AlertMessage}[/]");

					AnsiConsole.WriteLine();
				}
			}
			finally
			{
				_baselineFileMonitor.StatusChanged -= OnStatusChanged;
			}
			return;
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
			else CheckContinuously(settings.Baseline, settings.Interval, settings.Once, cancellationToken);
			return 0;
		}
	}
}
