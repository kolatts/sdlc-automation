using System.CommandLine;
using Spectre.Console;

namespace SdlcAutomation.Commands;

public abstract class BaseCommand : Command
{
    protected BaseCommand(string name, string? description = null)
        : base(name, description)
    {
    }

    protected void WriteSuccess(string message)
    {
        AnsiConsole.MarkupLine($"[green]✓[/] {message}");
    }

    protected void WriteError(string message)
    {
        AnsiConsole.MarkupLine($"[red]✗[/] {message}");
    }

    protected void WriteInfo(string message)
    {
        AnsiConsole.MarkupLine($"[blue]ℹ[/] {message}");
    }

    protected void WriteWarning(string message)
    {
        AnsiConsole.MarkupLine($"[yellow]⚠[/] {message}");
    }
}
