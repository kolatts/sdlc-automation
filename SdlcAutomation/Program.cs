using System.CommandLine;
using SdlcAutomation.Services;
using RootCmd = SdlcAutomation.Commands.RootCommand;

// Initialize the execution timer
ExecutionTimer.Initialize();

var rootCommand = new RootCmd();

var exitCode = await rootCommand.InvokeAsync(args);

// Stop the timer and show total execution time if enabled
ExecutionTimer.Stop();

return exitCode;
