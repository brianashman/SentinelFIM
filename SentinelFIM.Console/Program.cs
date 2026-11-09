using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SentinelFIM.Console.Commands;
using SentinelFIM.Core.Baseline;
using SentinelFIM.Core.Hashing;
using SentinelFIM.Core.Models;
using SentinelFIM.Infrastructure.Baseline.Creator;
using SentinelFIM.Infrastructure.Baseline.Store;
using SentinelFIM.Infrastructure.Hashing;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging();


#region Services
//Infrastructure & Core Services
builder.Services.AddSingleton<IHashProvider, SHA256HashProvider>();
builder.Services.AddSingleton<IBaselineStore, JsonBaselineStore>();
builder.Services.AddSingleton<BaselineCreator>();
//File provider name conflicts with an existing IFileProvider and FileSystemProvider
//Used the full namespace for the interface as a compensating control
builder.Services.AddSingleton<SentinelFIM.Core.Files.IFileProvider, SentinelFIM.Infrastructure.Files.FileSystemProvider>();

//CLI Commands
builder.Services.AddTransient<CreateBaselineCommand>();


//Just temporarily for testing, will be removed later
builder.Services.AddSingleton<SentinelFIM.Console.Temporary.TempLogger>();
#endregion


var host = builder.Build();

//Note to remove this later, this is just for testing the baseline creation command. Logger only has one parameter for now.
var runcommand = host.Services.GetRequiredService<CreateBaselineCommand>();
var tempLogger = host.Services.GetRequiredService<SentinelFIM.Console.Temporary.TempLogger>();

string path = @"C:\Users\beash\Test";
string outputFile = "baseline.json";
try
{
	runcommand.Execute(path, outputFile, BaselineScanType.Directory);
}
catch(Exception ex)
{
	tempLogger.LogError($"Error details: {ex.Message}");
}
