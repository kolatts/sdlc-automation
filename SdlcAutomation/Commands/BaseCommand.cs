using System.CommandLine;
using System.Diagnostics;
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

    /// <summary>
    /// Execute an operation with timing tracking
    /// </summary>
    protected async Task<T> ExecuteWithTimingAsync<T>(string operationName, Func<Task<T>> operation)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            WriteInfo($"Starting: {operationName}");
            var result = await operation();
            stopwatch.Stop();
            WriteSuccess($"Completed: {operationName} (took {stopwatch.ElapsedMilliseconds}ms)");
            return result;
        }
        catch
        {
            stopwatch.Stop();
            WriteError($"Failed: {operationName} (took {stopwatch.ElapsedMilliseconds}ms)");
            throw;
        }
    }

    /// <summary>
    /// Execute an operation with timing tracking (synchronous version)
    /// </summary>
    protected T ExecuteWithTiming<T>(string operationName, Func<T> operation)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            WriteInfo($"Starting: {operationName}");
            var result = operation();
            stopwatch.Stop();
            WriteSuccess($"Completed: {operationName} (took {stopwatch.ElapsedMilliseconds}ms)");
            return result;
        }
        catch
        {
            stopwatch.Stop();
            WriteError($"Failed: {operationName} (took {stopwatch.ElapsedMilliseconds}ms)");
            throw;
        }
    }

    /// <summary>
    /// Execute an operation with timing tracking (void async version)
    /// </summary>
    protected async Task ExecuteWithTimingAsync(string operationName, Func<Task> operation)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            WriteInfo($"Starting: {operationName}");
            await operation();
            stopwatch.Stop();
            WriteSuccess($"Completed: {operationName} (took {stopwatch.ElapsedMilliseconds}ms)");
        }
        catch
        {
            stopwatch.Stop();
            WriteError($"Failed: {operationName} (took {stopwatch.ElapsedMilliseconds}ms)");
            throw;
        }
    }
}
