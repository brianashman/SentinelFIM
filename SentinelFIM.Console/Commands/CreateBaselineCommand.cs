using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using SentinelFIM.Console.Commands.Settings;
using SentinelFIM.Core.Baseline;
using SentinelFIM.Core.Models;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;

namespace SentinelFIM.Console.Commands
{
	public class CreateBaselineCommand: Command<CreateBaselineSettings>
	{
		private readonly IBaselineStore _baselineStore;
		private readonly ILogger<CreateBaselineCommand> _logger;
		private readonly Core.Files.IFileProvider _fileProvider;
		public CreateBaselineCommand(IBaselineStore baselineStore, ILogger<CreateBaselineCommand> logger, Core.Files.IFileProvider fileProvider)
		{
			_baselineStore = baselineStore;
			_logger = logger;
			_fileProvider = fileProvider;
		}
		private void SaveBaseline(string path, string outputFile) => _baselineStore.SaveBaseline(path, outputFile);
		private void PreviewBaseline(string path)
		{
			Stopwatch timer = new();
			timer.Start();
			//No additional logs needed here, preview is console only.
			_logger.LogInformation("Previewing baseline creation for path: {Path}", path);
			List<string> values = new();

			//Reminder to add specific file provider to scan. (For Factory)
			AnsiConsole.MarkupLine($"[yellow]Baseline preview for path: {path}[/]");
			AnsiConsole.Status()
				.Spinner(Spinner.Known.BetaWave)
				.Start("Enumerating files...", ctx =>
				{
					values = [.. _fileProvider.EnumeratePaths(path, EnumerationSearchOption.AllDirectories)];
					ctx.Status("Done");
					// Add a small delay for testing purposes
					Task.Delay(500).Wait();

					timer.Stop();
					AnsiConsole.MarkupLine($"[yellow]Time taken: [bold]{timer.Elapsed}[/]");
					AnsiConsole.MarkupLine($"[yellow]Files processed: [bold]{values.Count}[/]");
				});
			AnsiConsole.MarkupLine($"[green]Preview complete.[/]");
		}

		public override int Execute(CommandContext context, CreateBaselineSettings settings, CancellationToken cancellationToken)
		{
			if (settings.Preview)
			{
				PreviewBaseline(settings.Path);
			}
			else
			{
				//removed for incremental testing purposes, only preview functionality is being tested. Uncomment to enable baseline creation.
				//SaveBaseline(settings.Path, settings.OutputFile);
			}
			return 0;
		}
	}
}
