namespace Chickensoft.GodotTestDriver.Input;

using System.Threading.Tasks;
using Godot;
using GodotTestDriver.Util;
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
    public static async Task ClickMouseAt(this Viewport viewport, Vector2 position, MouseButton button = MouseButton.Left)
    {
        await viewport.PressMouseAt(position, button);
        await viewport.ReleaseMouseAt(position, button);
    }

    /// <summary>
    /// Moves the mouse to the specified position.
    /// </summary>
    /// <param name="viewport">Viewport.</param>
    /// <param name="position">Position, in viewport coordinates.</param>
    /// <returns>Task that completes when the input finishes.</returns>
    public static async Task MoveMouseTo(this Viewport viewport, Vector2 position)
    {
        await viewport.ProcessFrame();

        viewport.WarpMouse(position);
        var inputEvent = new InputEventMouseMotion
        {
            GlobalPosition = position,
            Position = position
        };
        Input.ParseInputEvent(inputEvent);

        await viewport.WaitForEvents();
    }

    /// <summary>
    /// Drags the mouse from the start position to the end position.
    /// </summary>
    /// <param name="viewport">Viewport.</param>
    /// <param name="start">Start position, in viewport coordinates.</param>
    /// <param name="end">End position, in viewport coordinates.</param>
    /// <param name="button">Mouse button.</param>
    /// <returns>Task that completes when the input finishes.</returns>
    public static async Task DragMouse(this Viewport viewport, Vector2 start, Vector2 end, MouseButton button = MouseButton.Left)
    {
        await viewport.PressMouseAt(start, button);
        await viewport.ReleaseMouseAt(end, button);
    }

    /// <summary>
    /// Presses the given mouse button.
    /// </summary>
    /// <param name="viewport">Viewport.</param>
    /// <param name="button">Mouse button (left by default).</param>
    /// <returns>Task that completes when the input finishes.</returns>
    public static async Task PressMouse(this Viewport viewport, MouseButton button = MouseButton.Left)
    {
        await viewport.ProcessFrame();

        var action = new InputEventMouseButton
        {
            ButtonIndex = button,
            Pressed = true
        };
        Input.ParseInputEvent(action);

        await viewport.WaitForEvents();
    }

    /// <summary>
    /// Releases the given mouse button.
    /// </summary>
    /// <param name="viewport">Viewport.</param>
    /// <param name="button">Mouse button (left by default).</param>
    /// <returns>Task that completes when the input finishes.</returns>
    public static async Task ReleaseMouse(this Viewport viewport, MouseButton button = MouseButton.Left)
    {
        await viewport.ProcessFrame();

        var action = new InputEventMouseButton
        {
            ButtonIndex = button,
            Pressed = false
        };
        Input.ParseInputEvent(action);

        await viewport.WaitForEvents();
    }

    private static async Task PressMouseAt(this Viewport viewport, Vector2 position, MouseButton button = MouseButton.Left)
    {
        await MoveMouseTo(viewport, position);

        var action = new InputEventMouseButton
        {
            ButtonIndex = button,
            Pressed = true,
            Position = position
        };
        Input.ParseInputEvent(action);

        await viewport.WaitForEvents();
    }

    private static async Task ReleaseMouseAt(this Viewport viewport, Vector2 position, MouseButton button = MouseButton.Left)
    {
        await MoveMouseTo(viewport, position);

        var action = new InputEventMouseButton
        {
            ButtonIndex = button,
            Pressed = false,
            Position = position
        };
        Input.ParseInputEvent(action);

        await viewport.WaitForEvents();
    }
}
