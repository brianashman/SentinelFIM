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
		private void DisplayEnumerationResults(IReadOnlyList<string> values, int count, string path)
		{
			var directoryCount = values
				.Select(Path.GetDirectoryName)
				.Where(dir => !string.IsNullOrEmpty(dir))
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.Count();

			var table = new Table()
				.RoundedBorder()
				.BorderColor(Color.Green)
				.AddColumn("Metric")
				.AddColumn(new TableColumn("Value").RightAligned())
				.AddColumn(new TableColumn("Description").LeftAligned());

			table.AddRow("Files discovered", count.ToString("N0"), "The total number of files found in the specified path.");
			table.AddRow("Directories discovered", directoryCount.ToString("N0"), "The total number of directories found in the specified path.");
			AnsiConsole.MarkupLine($"[bold yellow]Enumeration Results:[/]");
			AnsiConsole.Write(table);

			AnsiConsole.WriteLine();
			AnsiConsole.MarkupLine($"[bold yellow]Directory Tree:[/]");
			//Note to implement later
			//var tree = new Tree($"[blue]{Markup.Escape(path)} [/]");

			AnsiConsole.Write(tree);

		}
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
				.Spinner(Spinner.Known.Shark)
				.SpinnerStyle(Style.Parse("green"))
				.Start("Enumerating files...", ctx =>
				{
					values = [.. _fileProvider.EnumeratePaths(path, EnumerationSearchOption.AllDirectories)];
					Task.Delay(100).Wait(); // Add a small delay for testing purposes
					ctx.Status("Done");
					// Add a small delay for testing purposes
					Task.Delay(100).Wait();

					timer.Stop();
					AnsiConsole.WriteLine();


				});
			timer.Stop();
			var count = values.Count;

			// Display the time taken in a more readable format if the elapsed time is greater than 1 second
			var timeFormat = timer.ElapsedMilliseconds switch
			{
				> 1000 => $"[yellow]Time taken:[/] [bold]{timer.ElapsedMilliseconds / 1000.0:F2}[/] s",
				_ => $"[yellow]Time taken:[/] [bold]{timer.ElapsedMilliseconds}[/] ms",
			};

			AnsiConsole.MarkupLine(timeFormat);
			AnsiConsole.MarkupLine($"[yellow]Files processed:[/] [bold]{count}[/]");
			AnsiConsole.MarkupLine($"[green]Preview complete[/]");
			AnsiConsole.WriteLine();

			DisplayEnumerationResults(values, count, path);
			return;
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
