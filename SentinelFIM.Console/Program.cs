using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SentinelFIM.Console.Commands.BaselineCommands;
using SentinelFIM.Core.Baseline;
using SentinelFIM.Core.Hashing;
using SentinelFIM.Core.Models;
using SentinelFIM.Core.Monitoring.Baseline;
using SentinelFIM.Infrastructure.Baseline.Creator;
using SentinelFIM.Infrastructure.Baseline.Store;
using SentinelFIM.Infrastructure.Hashing;
using SentinelFIM.Infrastructure.Monitoring.Baseline;
using Spectre.Console.Cli;
using System.Linq.Expressions;


var builder = Host.CreateApplicationBuilder(args);

//builder.Services.AddLogging();
builder.Logging.ClearProviders();

#region Services
//Infrastructure & Core Services
builder.Services.AddSingleton<IHashProvider, SHA256HashProvider>();
builder.Services.AddSingleton<IBaselineStore, JsonBaselineStore>();
builder.Services.AddSingleton<BaselineCreator>();
builder.Services.AddSingleton<IBaselineFileMonitor, PollingBaselineFileMonitor>();
//File provider name conflicts with an existing IFileProvider and FileSystemProvider
//Used the full namespace for the interface as a compensating control
builder.Services.AddSingleton<SentinelFIM.Core.Files.IFileProvider, SentinelFIM.Infrastructure.Files.FileSystemProvider>();
#endregion

var registrar = new SentinelFIM.Console.Spectre.TypeRegistrar(builder.Services);
var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
	e.Cancel = true; 
	cts.Cancel();
};

var app = new CommandApp(registrar);
app.Configure(config =>
{
	config.CaseSensitivity(CaseSensitivity.None);
	config.SetApplicationVersion("0.1.0");
	config.SetApplicationName("sentinelfim");

	config.AddBranch("baseline", baseline =>
	{
		baseline.AddCommand<CreateBaselineCommand>("create")
			.WithDescription("Creates a baseline of the specified path")
			.WithExample("baseline create --Path", "C:\\path\\to\\scan",  "--output C:\\path\\to\\output.json", "--recursive");
		baseline.AddCommand<MonitorBaselineCommand>("monitor")
			.WithDescription("Monitors the specified baseline file for changes")
			.WithExample("monitor", "C:\\path\\to\\baseline.json");
	});
});

return app.Run(args, cts.Token);

