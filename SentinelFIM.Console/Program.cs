using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SentinelFIM.Console.Commands;
using SentinelFIM.Core.Baseline;
using SentinelFIM.Core.Hashing;
using SentinelFIM.Core.Models;
using SentinelFIM.Infrastructure.Baseline.Creator;
using SentinelFIM.Infrastructure.Baseline.Store;
using SentinelFIM.Infrastructure.Hashing;
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
//File provider name conflicts with an existing IFileProvider and FileSystemProvider
//Used the full namespace for the interface as a compensating control
builder.Services.AddSingleton<SentinelFIM.Core.Files.IFileProvider, SentinelFIM.Infrastructure.Files.FileSystemProvider>();
#endregion

var registrar = new SentinelFIM.Console.Spectre.TypeRegistrar(builder.Services);

var app = new CommandApp(registrar);
//Note to remove the testing command before release. It is only for testing purposes.
app.Configure(config =>
{
	config.SetApplicationName("sentinelfim");
	config.AddBranch("baseline", baseline =>
	{
		baseline.AddCommand<CreateBaselineCommand>("create")
			.WithDescription("Creates a baseline of the specified path")
			.WithExample("baseline create --Path", "C:\\path\\to\\scan",  "--output C:\\path\\to\\output.json");
	});
});

return app.Run(args);

