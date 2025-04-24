namespace Chickensoft.GodotTestDriver;

using System;
using GodotTestDriver.Util;
using JetBrains.Annotations;

/// <summary>
/// Godot test driver configuration class.
/// </summary>
[PublicAPI]
public static class Configuration
{
    /// <summary>
    /// Logging configuration instance.
    /// </summary>
    public static LoggingConfiguration Logging { get; } = new LoggingConfiguration();

    /// <summary>
    /// Logging configuration.
    /// </summary>
    public class LoggingConfiguration
    {
        /// <summary>
        /// Debug logger.
        /// </summary>
        public static Action<string, object[]> DebugLogger
        {
            get => Log.DebugLogger;
            set => Log.DebugLogger = value;
        }

        /// <summary>
        /// Info logger.
        /// </summary>
        public static Action<string, object[]> InfoLogger
        {
            get => Log.InfoLogger;
            set => Log.InfoLogger = value;
        }

        /// <summary>
        /// Error logger.
        /// </summary>
        public static Action<string, object[]> ErrorLogger
        {
            get => Log.ErrorLogger;
            set => Log.ErrorLogger = value;
        }
    }
}
