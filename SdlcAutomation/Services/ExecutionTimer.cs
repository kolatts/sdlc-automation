using System.Diagnostics;

namespace SdlcAutomation.Services;

/// <summary>
/// Static execution timer for tracking overall application execution time
/// </summary>
public static class ExecutionTimer
{
    private static readonly Stopwatch _stopwatch = new();
    private static bool _isInitialized;

    /// <summary>
    /// Initialize and start the execution timer
    /// </summary>
    public static void Initialize()
    {
        if (!_isInitialized)
        {
            _stopwatch.Start();
            _isInitialized = true;
        }
    }

    /// <summary>
    /// Get the elapsed time since initialization
    /// </summary>
    public static TimeSpan Elapsed => _stopwatch.Elapsed;

    /// <summary>
    /// Get the elapsed milliseconds since initialization
    /// </summary>
    public static long ElapsedMilliseconds => _stopwatch.ElapsedMilliseconds;

    /// <summary>
    /// Reset the timer
    /// </summary>
    public static void Reset()
    {
        _stopwatch.Reset();
        _isInitialized = false;
    }

    /// <summary>
    /// Stop the timer
    /// </summary>
    public static void Stop()
    {
        _stopwatch.Stop();
    }
}
