namespace Chickensoft.GodotTestDriver.Input;

using System.Threading.Tasks;
using Godot;
using GodotTestDriver.Util;
using JetBrains.Annotations;

/// <summary>
/// Extensions which allow to send keyboard inputs.
/// </summary>
[PublicAPI]
public static class KeyboardControlExtensions
{
    /// <summary>
    /// Presses the given key with the given modifiers.
    /// </summary>
    /// <param name="node">Node to perform input on.</param>
    /// <param name="key">Keyboard key.</param>
    /// <param name="control">Control modifier.</param>
    /// <param name="alt">Alt modifier.</param>
    /// <param name="shift">Shift modifier.</param>
    /// <param name="meta">Meta-key (Windows, Command) modifier.</param>
    /// <returns>Asynchronous task completed when key is released.</returns>
    public static async Task PressKey(
        this Node node,
        Key key,
        bool control = false,
        bool alt = false,
        bool shift = false,
        bool meta = false
    )
    {
        var inputEvent = new InputEventKey
        {
            Pressed = true,
            Keycode = key,
            AltPressed = alt,
            CtrlPressed = control,
            ShiftPressed = shift,
            MetaPressed = meta
        };

        Input.ParseInputEvent(inputEvent);

        await node.WaitForEvents();
    }

    /// <summary>
    /// Simulate a key being pressed for a certain amount of time.
    /// </summary>
    /// <param name="node">Node to perform input on.</param>
    /// <param name="seconds">Input duration, in seconds.</param>
    /// <param name="key">Keyboard key.</param>
    /// <param name="control">Control modifier.</param>
    /// <param name="alt">Alt modifier.</param>
    /// <param name="shift">Shift modifier.</param>
    /// <param name="meta">Meta-key (Windows, Command) modifier.</param>
    /// <returns>Asynchronous task completed when key is released.</returns>
    public static async Task HoldKeyFor(
        this Node node,
        float seconds,
        Key key,
        bool control = false,
        bool alt = false,
        bool shift = false,
        bool meta = false
    )
    {
        await node.PressKey(key, control, alt, shift, meta);
        await node.Wait(seconds);
        await node.ReleaseKey(key, control, alt, shift, meta);
    }

    /// <summary>
    /// Releases the given key with the given modifier state.
    /// </summary>
    /// <param name="node">Node to perform input on.</param>
    /// <param name="key">Keyboard key.</param>
    /// <param name="control">Control modifier.</param>
    /// <param name="alt">Alt modifier.</param>
    /// <param name="shift">Shift modifier.</param>
    /// <param name="meta">Meta-key (Windows, Command) modifier.</param>
    /// <returns>Asynchronous task completed when key is released.</returns>
    public static async Task ReleaseKey(
        this Node node,
        Key key,
        bool control = false,
        bool alt = false,
        bool shift = false,
        bool meta = false
    )
    {
        var inputEvent = new InputEventKey
        {
            Pressed = false,
            Keycode = key,
            CtrlPressed = control,
            AltPressed = alt,
            ShiftPressed = shift,
            MetaPressed = meta
        };

        Input.ParseInputEvent(inputEvent);

        await node.WaitForEvents();
    }

    /// <summary>
    /// Presses and releases a key with the given modifiers.
    /// </summary>
    /// <param name="node">Node to perform input on.</param>
    /// <param name="key">Keyboard key.</param>
    /// <param name="control">Control modifier.</param>
    /// <param name="alt">Alt modifier.</param>
    /// <param name="shift">Shift modifier.</param>
    /// <param name="meta">Meta-key (Windows, Command) modifier.</param>
    /// <returns>Asynchronous task completed when key is released.</returns>
    public static async Task TypeKey(
        this Node node,
        Key key,
        bool control = false,
        bool alt = false,
        bool shift = false,
        bool meta = false
    )
    {
        await node.PressKey(key, control, alt, shift, meta);
        await node.ReleaseKey(key, control, alt, shift, meta);
    }
}
