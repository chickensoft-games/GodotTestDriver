namespace Chickensoft.GodotTestDriver.Input;

using System;
using Godot;
using JetBrains.Annotations;

/// <summary>
/// Extensions for simulating controller inputs.
/// </summary>
[PublicAPI]
public static class ControllerInputExtensions
{
    /// <summary>
    /// Set a simulated input value for a two-sided analog axis on a controller (for instance, the x-axis of a thumbstick).
    /// </summary>
    /// <param name="node">Node that generates simulated input.</param>
    /// <param name="negativeAction">Action name for the negative side of the axis.</param>
    /// <param name="positiveAction">Action name for the positive side of the axis.</param>
    /// <param name="axisPosition">The position of the axis input, from -1 to +1.</param>
    /// <seealso cref="Input.GetAxis(StringName, StringName)"/>
    public static void SetBidirectionalAxisInput(this Node node, string negativeAction, string positiveAction, float axisPosition)
    {
        var onAction = axisPosition >= 0 ? positiveAction : negativeAction;
        var offAction = axisPosition >= 0 ? negativeAction : positiveAction;
        axisPosition = Math.Abs(axisPosition);
        node.StartAction(onAction, axisPosition);
        node.EndAction(offAction);
    }

    /// <summary>
    /// End a simulated input for a two-sided analog axis on a controller (for instance, the x-axis of a thumbstick).
    /// </summary>
    /// <param name="node">Node that generates simulated input.</param>
    /// <param name="negativeAction">Action name for the negative side of the axis.</param>
    /// <param name="positiveAction">Action name for the positive side of the axis.</param>
    public static void EndBidirectionalAxisInput(this Node node, string negativeAction, string positiveAction)
    {
        node.EndAction(negativeAction);
        node.EndAction(positiveAction);
    }

    /// <summary>
    /// Set a simulated input value for a one-sided analog axis on a controller (for instance, a gamepad trigger).
    /// </summary>
    /// <param name="node">Node that generates simulated input.</param>
    /// <param name="action">Action name for the axis.</param>
    /// <param name="axisPosition">The position of the axis input, from 0 to 1.</param>
    /// <seealso cref="Input.GetActionStrength(StringName, bool)"/>
    public static void SetSingleAxisInput(this Node node, string action, float axisPosition)
    {
        node.StartAction(action, axisPosition);
    }

    /// <summary>
    /// End a simulated input for a one-sided analog axis on a controller (for instance, a gamepad trigger).
    /// </summary>
    /// <param name="node">Node that generates simulated input.</param>
    /// <param name="action">Action name for the axis.</param>
    public static void EndSingleAxisInput(this Node node, string action)
    {
        node.EndAction(action);
    }
}
