namespace Chickensoft.GodotTestDriver.Util;

using System;
using Godot;

internal static class Log
{
    public static Action<string, object[]> DebugLogger { get; set; } = (s, p) => GD.Print("DEBUG: " + string.Format(s, p));
    public static Action<string, object[]> InfoLogger { get; set; } = (s, p) => GD.Print("INFO: " + string.Format(s, p));
    public static Action<string, object[]> ErrorLogger { get; set; } = (s, p) => GD.PrintErr("ERROR: " + string.Format(s, p));

    public static void Debug(string format, params object[] args)
    {
        DebugLogger?.Invoke(format, args);
    }

    public static void Info(string format, params object[] args)
    {
        InfoLogger?.Invoke(format, args);
    }

    public static void Error(string format, params object[] args)
    {
        ErrorLogger?.Invoke(format, args);
    }
}
