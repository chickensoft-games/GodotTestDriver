namespace Chickensoft.GodotTestDriver.Input;

using System.Threading.Tasks;
using Chickensoft.GodotTestDriver.Util;
using Godot;
using JetBrains.Annotations;

/// <summary>
/// Extensions for simulating controller inputs.
/// </summary>
/// <seealso cref="ActionsInputExtensions"/>
[PublicAPI]
public static class ControllerInputExtensions
{
    /// <summary>
    /// Holds a controller axis at a given position for a given period of time before releasing
    /// it, causing <see cref="InputEventJoypadMotion"/> events to fire at an appropriate interval.
    /// </summary>
    /// <remarks>
    /// Does not affect the values of <see cref="Input.IsActionPressed(StringName, bool)"/>,
    /// <see cref="Input.GetJoyAxis(int, JoyAxis)"/>, or <see cref="Input.GetActionStrength(StringName, bool)"/>.
    /// </remarks>
    /// <param name="node">Node that generates simulated input.</param>
    /// <param name="seconds">Input duration, in seconds.</param>
    /// <param name="axis">The controller axis to set.</param>
    /// <param name="axisValue">The axis position, in the range -1.0f to 1.0f.</param>
    /// <param name="device">Input device that is the source of the event.</param>
    /// <returns>Asynchronous task completed when the button is released.</returns>
    /// <seealso cref="MoveJoypadAxisTo(Node, JoyAxis, float, int)"/>
    public static async Task HoldJoypadAxisFor(this Node node, float seconds, JoyAxis axis, float axisValue, int device = 0)
    {
        node.MoveJoypadAxisTo(axis, axisValue, device);
        await node.Wait(seconds);
        node.ReleaseJoypadAxis(axis, device);
    }

    /// <summary>
    /// Holds a controller button down for a given period of time before releasing it, causing
    /// <see cref="InputEventJoypadButton"/> events to fire at an appropriate interval.
    /// </summary>
    /// <remarks>
    /// Does not affect the value of <see cref="Input.IsActionPressed(StringName, bool)"/>
    /// or <see cref="Input.IsJoyButtonPressed(int, JoyButton)"/>.
    /// </remarks>
    /// <param name="node">Node that generates simulated input.</param>
    /// <param name="seconds">Input duration, in seconds.</param>
    /// <param name="buttonIndex">Button that will be pressed.</param>
    /// <param name="device">Input device that is the source of the event.</param>
    /// <param name="pressure">Pressure on the button, in the range 0.0f to 1.0f.</param>
    /// <returns>Asynchronous task completed when the button is released.</returns>
    public static async Task HoldJoypadButtonFor(this Node node, float seconds, JoyButton buttonIndex, int device = 0, float pressure = 1.0f)
    {
        node.PressJoypadButton(buttonIndex, device, pressure);
        await node.Wait(seconds);
        node.ReleaseJoypadButton(buttonIndex, device);
    }

    /// <summary>
    /// Presses and releases a controller button, causing <see cref="InputEventJoypadButton"/>
    /// events to fire.
    /// </summary>
    /// <remarks>
    /// Does not affect the value of <see cref="Input.IsActionPressed(StringName, bool)"/>
    /// or <see cref="Input.IsJoyButtonPressed(int, JoyButton)"/>.
    /// </remarks>
    /// <param name="node">Node that generates simulated input.</param>
    /// <param name="buttonIndex">Button that will be pressed.</param>
    /// <param name="device">Input device that is the source of the event.</param>
    /// <param name="pressure">Pressure on the button, in the range 0.0f to 1.0f.</param>
    public static void TapJoypadButton(this Node node, JoyButton buttonIndex, int device = 0, float pressure = 1.0f)
    {
        node.PressJoypadButton(buttonIndex, device, pressure);
        node.ReleaseJoypadButton(buttonIndex, device);
    }

    /// <summary>
    /// Set a controller axis to a given position, causing a <see cref="InputEventJoypadMotion"/>
    /// to fire.
    /// </summary>
    /// <remarks>
    /// Although the full valid range of <paramref name="axisValue"/> is -1.0f to 1.0f,
    /// some controller axes (e.g., gamepad triggers) only generate values between 0.0f
    /// and 1.0f. Does not affect the values of <see cref="Input.IsActionPressed(StringName, bool)"/>,
    /// <see cref="Input.GetJoyAxis(int, JoyAxis)"/>, or <see cref="Input.GetActionStrength(StringName, bool)"/>.
    /// </remarks>
    /// <param name="_">Node that generates simulated input.</param>
    /// <param name="axis">The controller axis to set.</param>
    /// <param name="axisValue">The axis position, in the range -1.0f to 1.0f.</param>
    /// <param name="device">Input device that is the source of the event.</param>
    public static void MoveJoypadAxisTo(this Node _, JoyAxis axis, float axisValue, int device = 0)
    {
        var inputEvent = new InputEventJoypadMotion
        {
            Axis = axis,
            AxisValue = axisValue,
            Device = device
        };
        Input.ParseInputEvent(inputEvent);
        Input.FlushBufferedEvents();
    }

    /// <summary>
    /// Release a controller axis, setting it to its rest position, causing a
    /// <see cref="InputEventJoypadMotion"/> to fire.
    /// </summary>
    /// <remarks>
    /// Equivalent to <c>node.MoveJoypadAxisTo(axis, 0.0f, device)</c>. Does not affect
    /// the values of <see cref="Input.IsActionPressed(StringName, bool)"/>,
    /// <see cref="Input.GetJoyAxis(int, JoyAxis)"/>, or <see cref="Input.GetActionStrength(StringName, bool)"/>.
    /// </remarks>
    /// <param name="node">Node that generates simulated input.</param>
    /// <param name="axis">The controller axis to release.</param>
    /// <param name="device">Input device that is the source of the event.</param>
    /// <seealso cref="MoveJoypadAxisTo(Node, JoyAxis, float, int)"/>
    public static void ReleaseJoypadAxis(this Node node, JoyAxis axis, int device = 0)
    {
        node.MoveJoypadAxisTo(axis, 0.0f, device);
    }

    /// <summary>
    /// Presses a controller button, causing an <see cref="InputEventJoypadButton"/> to fire.
    /// </summary>
    /// <remarks>
    /// Does not affect the values of <see cref="Input.IsActionPressed(StringName, bool)"/>
    /// or <see cref="Input.IsJoyButtonPressed(int, JoyButton)"/>.
    /// </remarks>
    /// <param name="_">Node that generates simulated input.</param>
    /// <param name="buttonIndex">Button that will be pressed.</param>
    /// <param name="device">Input device that is the source of the event.</param>
    /// <param name="pressure">Pressure on the button, in the range 0.0f to 1.0f.</param>
    public static void PressJoypadButton(this Node _, JoyButton buttonIndex, int device = 0, float pressure = 1.0f)
    {
        var inputEvent = new InputEventJoypadButton
        {
            Pressed = true,
            ButtonIndex = buttonIndex,
            Pressure = pressure,
            Device = device
        };
        Input.ParseInputEvent(inputEvent);
        Input.FlushBufferedEvents();
    }

    /// <summary>
    /// Releases a controller button, causing an <see cref="InputEventJoypadButton"/> to fire.
    /// </summary>
    /// <remarks>
    /// Does not affect the values of <see cref="Input.IsActionPressed(StringName, bool)"/>
    /// or <see cref="Input.IsJoyButtonPressed(int, JoyButton)"/>.
    /// </remarks>
    /// <param name="_">Node that generates simulated input.</param>
    /// <param name="buttonIndex">Button that will be released.</param>
    /// <param name="device">Input device that is the source of the event.</param>
    public static void ReleaseJoypadButton(this Node _, JoyButton buttonIndex, int device = 0)
    {
        var inputEvent = new InputEventJoypadButton
        {
            Pressed = false,
            ButtonIndex = buttonIndex,
            Pressure = 0.0f,
            Device = device
        };
        Input.ParseInputEvent(inputEvent);
        Input.FlushBufferedEvents();
    }
}
