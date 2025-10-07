using Spectre.Console;
using System.Diagnostics;

namespace SdlcAutomation.Services;

/// <summary>
/// Wrapper for console logging with spinner support and timing tracking
/// </summary>
public class ConsoleLogger
{
    /// <summary>
    /// Write a success message
    /// </summary>
    public void WriteSuccess(string message)
    {
        AnsiConsole.MarkupLine($"[green]✓[/] {Markup.Escape(message)}");
    }

    /// <summary>
    /// Write an error message
    /// </summary>
    public void WriteError(string message)
    {
        AnsiConsole.MarkupLine($"[red]✗[/] {Markup.Escape(message)}");
    }

    /// <summary>
    /// Write an info message
    /// </summary>
    public void WriteInfo(string message)
    {
        AnsiConsole.MarkupLine($"[blue]ℹ[/] {Markup.Escape(message)}");
    }

    /// <summary>
    /// Write a warning message
    /// </summary>
    public void WriteWarning(string message)
    {
        AnsiConsole.MarkupLine($"[yellow]⚠[/] {Markup.Escape(message)}");
    }

    /// <summary>
    /// Execute an operation with a spinner display and timing tracking
    /// </summary>
    public async Task<T> ExecuteWithSpinnerAsync<T>(string operationName, Func<Task<T>> operation)
    {
        var stopwatch = Stopwatch.StartNew();
        var startTime = ExecutionTimer.ElapsedMilliseconds;
        
        try
        {
            return await AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .SpinnerStyle(Style.Parse("blue"))
                .StartAsync($"[blue]{Markup.Escape(operationName)}...[/]", async ctx =>
                {
                    var result = await operation();
                    stopwatch.Stop();
                    
                    ctx.Status($"[green]✓[/] {Markup.Escape(operationName)}");
                    ctx.Refresh();
                    
                    return result;
                });
        }
        finally
        {
            stopwatch.Stop();
            var totalTime = ExecutionTimer.ElapsedMilliseconds;
            WriteSuccess($"{operationName} completed in {stopwatch.ElapsedMilliseconds}ms (total: {totalTime}ms)");
        }
    }

    /// <summary>
    /// Execute an operation with a spinner display and timing tracking (void return)
    /// </summary>
    public async Task ExecuteWithSpinnerAsync(string operationName, Func<Task> operation)
    {
        await ExecuteWithSpinnerAsync<object?>(operationName, async () =>
        {
            await operation();
            return null;
        });
    }

    /// <summary>
    /// Execute a synchronous operation with a spinner display and timing tracking
    /// </summary>
    public T ExecuteWithSpinner<T>(string operationName, Func<T> operation)
    {
        var stopwatch = Stopwatch.StartNew();
        var startTime = ExecutionTimer.ElapsedMilliseconds;
        
        try
        {
            return AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .SpinnerStyle(Style.Parse("blue"))
                .Start($"[blue]{Markup.Escape(operationName)}...[/]", ctx =>
                {
                    var result = operation();
                    stopwatch.Stop();
                    
                    ctx.Status($"[green]✓[/] {Markup.Escape(operationName)}");
                    ctx.Refresh();
                    
                    return result;
                });
        }
        finally
        {
            stopwatch.Stop();
            var totalTime = ExecutionTimer.ElapsedMilliseconds;
            WriteSuccess($"{operationName} completed in {stopwatch.ElapsedMilliseconds}ms (total: {totalTime}ms)");
        }
    }

    /// <summary>
    /// Execute a synchronous operation with a spinner display and timing tracking (void return)
    /// </summary>
    public void ExecuteWithSpinner(string operationName, Action operation)
    {
        ExecuteWithSpinner<object?>(operationName, () =>
        {
            operation();
            return null;
        });
    }
}
