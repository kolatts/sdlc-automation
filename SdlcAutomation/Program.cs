using System.CommandLine;
using RootCmd = SdlcAutomation.Commands.RootCommand;

var rootCommand = new RootCmd();

return await rootCommand.InvokeAsync(args);
