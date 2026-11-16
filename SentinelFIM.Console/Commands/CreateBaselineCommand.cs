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
	//Reminder to fix preview command always enumerating recursively, even when the recursive option is not specified.
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
		private BaselineResults SaveBaseline(string path, string outputFile, bool isRecursive) => _baselineStore.SaveBaseline(path, outputFile, isRecursive);
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
			table.AddRow("Directories discovered", directoryCount.ToString("N0"), "The total number of directories included in enumeration.");
			AnsiConsole.MarkupLine($"[bold yellow]Enumeration Results:[/]");
			AnsiConsole.Write(table);

			AnsiConsole.WriteLine();
			//Note to implement later

			var directories = Directory.EnumerateDirectories(path, "*", SearchOption.AllDirectories);
			if (directories == null) return;

			AnsiConsole.MarkupLine($"[bold yellow]Directory Tree:[/]");
			var tree = new Tree($"[blue]{Markup.Escape(path)} [/]");
			foreach (var item in directories!)
			{
				var subDirectory = Path.GetFullPath(item);
				tree.AddNode($"[blue]{Markup.Escape(subDirectory.ToString()!)}[/]");
			}

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
					//Task.Delay(100).Wait(); // Add a small delay for testing purposes
					ctx.Status("Done");
					// Add a small delay for testing purposes
					//Task.Delay(100).Wait();

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
		public override ValidationResult Validate(CommandContext context, CreateBaselineSettings settings)
		{
			if (string.IsNullOrWhiteSpace(settings.OutputFile) && !settings.Preview) 
			{
				var result = ValidationResult.Error("" +
				"Output Path must be specified with -o or --output");
				AnsiConsole.MarkupLine("[yellow]Use -h or --help for more options[/]");
				return result;
			}
			else return ValidationResult.Success();
		}
		public void CreateBaseline(CreateBaselineSettings settings)
		{
			var results = SaveBaseline(settings.Path, settings.OutputFile, settings.IsRecursive);
			if (results.FilesSaved == 0) AnsiConsole.MarkupLine("[yellow]No Files Saved[/]");
		}
		public override int Execute(CommandContext context, CreateBaselineSettings settings, CancellationToken cancellationToken)
		{
			if (settings.Preview) PreviewBaseline(settings.Path);
			else CreateBaseline(settings);
			return 0;
		}
	}
}
