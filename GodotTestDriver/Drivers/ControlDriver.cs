namespace Chickensoft.GodotTestDriver.Drivers;

using System;
using System.Threading.Tasks;
using Godot;
using GodotTestDriver.Input;
using GodotTestDriver.Util;
using JetBrains.Annotations;

/// <summary>
/// Driver for <see cref="Control"/> nodes.
/// </summary>
/// <typeparam name="T">Control type.</typeparam>
[PublicAPI]
public class ControlDriver<T> : CanvasItemDriver<T> where T : Control
{
    /// <summary>
    /// Creates a new generic ControlDriver.
    /// </summary>
    /// <param name="producer">Producer that creates a Control subclass.</param>
    /// <param name="description">Driver description.</param>
    public ControlDriver(Func<T> producer, string description = "") : base(producer, description)
    {
    }

    /// <summary>
    /// Returns true if the Node is visible and fully inside the viewport rect.
    /// </summary>
    public bool IsFullyInView
    {
        get
        {
            if (!IsVisible)
            {
                return false;
            }

            var control = VisibleRoot;
            var screenRect = control.GetViewport().WorldToScreen(control.GetGlobalRect());
            return control.GetViewportRect().Encloses(screenRect);
        }
    }

    /// <summary>
    /// Clicks the control with the mouse in the center.
    /// </summary>
    /// <param name="button">Mouse button.</param>
    public virtual async Task ClickCenter(MouseButton button = MouseButton.Left)
    {
        var control = VisibleRoot;
        await control.GetViewport().ClickMouseAt(control.GetGlobalRect().Center(), button);
    }

    /// <summary>
    /// Moves the mouse to the center of the control and hovers for the given amount
    /// of seconds.
    /// </summary>
    /// <param name="seconds">Number of seconds.</param>
    public async Task Hover(float seconds)
    {
        var control = VisibleRoot;
        await control.GetViewport().MoveMouseTo(control.GetGlobalRect().Center());
        await control.SleepSeconds(seconds);
    }

    /// <summary>
    /// Instructs the control to release the focus.
    /// </summary>
    public async Task ReleaseFocus()
    {
        var control = VisibleRoot;
        await control.GetTree().NextFrame();
        control.ReleaseFocus();
        await control.GetTree().WaitForEvents();
    }

    /// <summary>
    /// Instructs the control to grab the focus.
    /// </summary>
    public async Task GrabFocus()
    {
        var control = VisibleRoot;
        await control.GetTree().NextFrame();
        control.GrabFocus();
        await control.GetTree().WaitForEvents();
    }
}
