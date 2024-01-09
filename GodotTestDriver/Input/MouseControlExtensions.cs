namespace Chickensoft.GodotTestDriver.Input;

using Godot;
using JetBrains.Annotations;

/// <summary>
/// Extension functionality for controlling the mouse from tests.
/// </summary>
[PublicAPI]
public static class MouseControlExtensions
{
    /// <summary>
    /// Clicks the mouse at the specified position.
    /// </summary>
    /// <param name="viewport">Viewport.</param>
    /// <param name="position">Position, in viewport coordinates.</param>
    /// <param name="button">Mouse button.</param>
    /// <returns>Task that completes when the input finishes.</returns>
    public static void ClickMouseAt(this Viewport viewport, Vector2 position, MouseButton button = MouseButton.Left)
    {
        viewport.PressMouseAt(position, button);
        viewport.ReleaseMouseAt(position, button);
    }

    /// <summary>
    /// Moves the mouse to the specified position.
    /// </summary>
    /// <param name="viewport">Viewport.</param>
    /// <param name="position">Position, in viewport coordinates.</param>
    /// <returns>Task that completes when the input finishes.</returns>
    public static void MoveMouseTo(this Viewport viewport, Vector2 position)
    {
        var oldPosition = viewport.GetMousePosition();
        viewport.WarpMouse(position);
        var inputEvent = new InputEventMouseMotion
        {
            GlobalPosition = position,
            Position = position,
            Relative = position - oldPosition
        };
        Input.ParseInputEvent(inputEvent);
        Input.FlushBufferedEvents();
    }

    /// <summary>
    /// Drags the mouse from the start position to the end position.
    /// </summary>
    /// <param name="viewport">Viewport.</param>
    /// <param name="start">Start position, in viewport coordinates.</param>
    /// <param name="end">End position, in viewport coordinates.</param>
    /// <param name="button">Mouse button.</param>
    /// <returns>Task that completes when the input finishes.</returns>
    public static void DragMouse(this Viewport viewport, Vector2 start, Vector2 end, MouseButton button = MouseButton.Left)
    {
        viewport.PressMouseAt(start, button);
        viewport.ReleaseMouseAt(end, button);
    }

    /// <summary>
    /// Presses the given mouse button.
    /// </summary>
    /// <param name="_">Viewport.</param>
    /// <param name="button">Mouse button (left by default).</param>
    /// <returns>Task that completes when the input finishes.</returns>
    public static void PressMouse(this Viewport _, MouseButton button = MouseButton.Left)
    {
        var action = new InputEventMouseButton
        {
            ButtonIndex = button,
            Pressed = true
        };
        Input.ParseInputEvent(action);
        Input.FlushBufferedEvents();
    }

    /// <summary>
    /// Releases the given mouse button.
    /// </summary>
    /// <param name="_">Viewport.</param>
    /// <param name="button">Mouse button (left by default).</param>
    /// <returns>Task that completes when the input finishes.</returns>
    public static void ReleaseMouse(this Viewport _, MouseButton button = MouseButton.Left)
    {
        var action = new InputEventMouseButton
        {
            ButtonIndex = button,
            Pressed = false
        };
        Input.ParseInputEvent(action);
        Input.FlushBufferedEvents();
    }

    private static void PressMouseAt(this Viewport viewport, Vector2 position, MouseButton button = MouseButton.Left)
    {
        viewport.MoveMouseTo(position);

        var action = new InputEventMouseButton
        {
            ButtonIndex = button,
            Pressed = true,
            Position = position
        };
        Input.ParseInputEvent(action);
        Input.FlushBufferedEvents();
    }

    private static void ReleaseMouseAt(this Viewport viewport, Vector2 position, MouseButton button = MouseButton.Left)
    {
        viewport.MoveMouseTo(position);

        var action = new InputEventMouseButton
        {
            ButtonIndex = button,
            Pressed = false,
            Position = position
        };
        Input.ParseInputEvent(action);
        Input.FlushBufferedEvents();
    }
}
