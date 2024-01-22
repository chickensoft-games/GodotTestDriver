namespace Chickensoft.GodotTestDriver.Input;

using System.Threading.Tasks;
using Godot;
using GodotTestDriver.Util;
using JetBrains.Annotations;

#pragma warning disable IDE0060

/// <summary>
/// Input action extensions.
/// </summary>
[PublicAPI]
public static class ActionsControlExtensions
{
    /// <summary>
    /// Hold an input action for a given duration.
    /// </summary>
    /// <param name="node">Node to supply input to.</param>
    /// <param name="seconds">Time, in seconds.</param>
    /// <param name="actionName">Name of the action.</param>
    /// <returns>Task that completes when the input finishes.</returns>
    public static async Task HoldActionFor(
        this Node node,
        float seconds,
        string actionName
    )
    {
        node.StartAction(actionName);
        await node.Wait(seconds);
        node.EndAction(actionName);
    }

    /// <summary>
    /// Start an input action.
    /// </summary>
    /// <param name="node">Node to supply input to.</param>
    /// <param name="actionName">Name of the action.</param>
    /// <param name="strength">Action strength (optional — default is 1.0).</param>
    public static void StartAction(
        this Node node, string actionName, float strength = 1f
    )
    {
        Input.ParseInputEvent(new InputEventAction
        {
            Action = actionName,
            Pressed = true
        });
        Input.ActionPress(actionName, strength);
        Input.FlushBufferedEvents();
    }

    /// <summary>
    /// End an input action.
    /// </summary>
    /// <param name="node">Node to supply input to.</param>
    /// <param name="actionName">Name of the action.</param>
    public static void EndAction(this Node node, string actionName)
    {
        Input.ParseInputEvent(new InputEventAction
        {
            Action = actionName,
            Pressed = false
        });
        Input.ActionRelease(actionName);
        Input.FlushBufferedEvents();
    }
}

#pragma warning restore IDE0060
