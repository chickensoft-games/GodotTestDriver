namespace Chickensoft.GodotTestDriver.Util;

using Godot;
using JetBrains.Annotations;

/// <summary>
/// Some useful extension methods. These are declared internal instead using GodotExt, so users can decide what
/// they want.
/// </summary>
internal static class InternalUtil
{
    /// <summary>
    /// Returns the center of the given rect.
    /// </summary>
    /// <param name="rect2">Rectangle to compute the center of.</param>
    public static Vector2 Center(this Rect2 rect2)
    {
        return rect2.Position + (rect2.Size / 2);
    }

    /// <summary>
    /// Sleeps for the given amount of seconds.
    /// </summary>
    /// <param name="source">Node in the scene tree.</param>
    /// <param name="sleepTime">Time to sleep, in seconds.</param>
    [MustUseReturnValue]
    public static SignalAwaiter SleepSeconds(this Node source, float sleepTime)
    {
        return source.ToSignal(source.GetTree().CreateTimer(sleepTime), SceneTreeTimer.SignalName.Timeout);
    }

    /// <summary>
    /// Converts the given rect to global coordinates.
    /// </summary>
    /// <param name="localRect">Local rectangle.</param>
    /// <param name="globalPosition">Global position.</param>
    public static Rect2 ToGlobalRect(this Rect2 localRect, Vector2 globalPosition)
    {
        return new Rect2(globalPosition + localRect.Position, localRect.Size);
    }

    /// <summary>
    /// Converts the given world position to viewport screen coordinates.
    /// </summary>
    /// <param name="viewport">Viewport.</param>
    /// <param name="worldPosition">World position.</param>
    public static Vector2 WorldToScreen(this Viewport viewport, Vector2 worldPosition)
    {
        return viewport.CanvasTransform * worldPosition;
    }

    /// <summary>
    /// Converts the given viewport screen coordinates to world position.
    /// </summary>
    /// <param name="viewport">Viewport.</param>
    /// <param name="screenPosition">Screen position.</param>
    public static Vector2 ScreenToWorld(this Viewport viewport, Vector2 screenPosition)
    {
        return screenPosition * viewport.CanvasTransform;
    }

    // same for Rect2

    /// <summary>
    /// Converts the given world rect to viewport screen coordinates.
    /// </summary>
    /// <param name="viewport">Viewport.</param>
    /// <param name="worldRect">Rectangle in world coordinates.</param>
    public static Rect2 WorldToScreen(this Viewport viewport, Rect2 worldRect)
    {
        return viewport.CanvasTransform * worldRect;
    }

    /// <summary>
    /// Converts the given viewport screen coordinates to world rect.
    /// </summary>
    /// <param name="viewport">Viewport.</param>
    /// <param name="screenRect">Rectangle in screen coordinates.</param>
    public static Rect2 ScreenToWorld(this Viewport viewport, Rect2 screenRect)
    {
        return screenRect * viewport.CanvasTransform;
    }
}
