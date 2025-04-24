namespace Chickensoft.GodotTestDriver.Util;

using System;

/// <summary>
/// Timeout mechanism that determines if a given span of time has elapsed.
/// </summary>
public readonly struct Timeout
{
    private readonly float _seconds;
    private readonly long _start;

    /// <summary>
    /// Create a new timeout with the given duration.
    /// </summary>
    /// <param name="seconds">Time, in seconds.</param>
    public Timeout(float seconds)
    {
        _seconds = seconds;
        _start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// Whether the given duration has elapsed.
    /// </summary>
    public bool IsReached => DateTimeOffset.Now.ToUnixTimeMilliseconds() - _start > 1000f * _seconds;
}
