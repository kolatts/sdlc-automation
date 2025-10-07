using System.CommandLine;
using SdlcAutomation.Services;

namespace SdlcAutomation.Commands;

public abstract class BaseCommand : Command
{
    protected readonly ConsoleLogger Logger = new();

    protected BaseCommand(string name, string? description = null)
        : base(name, description)
    {
    }

    /// <summary>
    /// Write a success message (backward compatibility)
    /// </summary>
    protected void WriteSuccess(string message) => Logger.WriteSuccess(message);

    /// <summary>
    /// Write an error message (backward compatibility)
    /// </summary>
    protected void WriteError(string message) => Logger.WriteError(message);

    /// <summary>
    /// Write an info message (backward compatibility)
    /// </summary>
    protected void WriteInfo(string message) => Logger.WriteInfo(message);

    /// <summary>
    /// Write a warning message (backward compatibility)
    /// </summary>
    protected void WriteWarning(string message) => Logger.WriteWarning(message);

    /// <summary>
    /// Execute an operation with timing tracking and spinner (async with return value)
    /// </summary>
    protected Task<T> ExecuteWithTimingAsync<T>(string operationName, Func<Task<T>> operation)
        => Logger.ExecuteWithSpinnerAsync(operationName, operation);

    /// <summary>
    /// Execute an operation with timing tracking and spinner (async void)
    /// </summary>
    protected Task ExecuteWithTimingAsync(string operationName, Func<Task> operation)
        => Logger.ExecuteWithSpinnerAsync(operationName, operation);

    /// <summary>
    /// Execute an operation with timing tracking and spinner (synchronous with return value)
    /// </summary>
    protected T ExecuteWithTiming<T>(string operationName, Func<T> operation)
        => Logger.ExecuteWithSpinner(operationName, operation);

    /// <summary>
    /// Execute an operation with timing tracking and spinner (synchronous void)
    /// </summary>
    protected void ExecuteWithTiming(string operationName, Action operation)
        => Logger.ExecuteWithSpinner(operationName, operation);
}
