namespace Chickensoft.GodotTestDriver.Input;

using System;
using System.Threading.Tasks;
using Chickensoft.GodotTestDriver.Util;
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
    public static void SetControllerDoubleAxisInput(this Node node, string negativeAction, string positiveAction, float axisPosition)
    {
        if (axisPosition == 0)
        {
            node.EndAction(negativeAction);
            node.EndAction(positiveAction);
        }
        else
        {
            var onAction = axisPosition >= 0 ? positiveAction : negativeAction;
            var offAction = axisPosition >= 0 ? negativeAction : positiveAction;
            axisPosition = Math.Abs(axisPosition);
            node.StartAction(onAction, axisPosition);
            node.EndAction(offAction);
        }
    }

    /// <summary>
    /// End a simulated input for a two-sided analog axis on a controller (for instance, the x-axis of a thumbstick).
    /// </summary>
    /// <param name="node">Node that generates simulated input.</param>
    /// <param name="negativeAction">Action name for the negative side of the axis.</param>
    /// <param name="positiveAction">Action name for the positive side of the axis.</param>
    public static void EndControllerDoubleAxisInput(this Node node, string negativeAction, string positiveAction)
    {
        node.EndAction(negativeAction);
        node.EndAction(positiveAction);
    }

    /// <summary>
    /// Hold a two-sided analog-axis controller input for a given duration.
    /// </summary>
    /// <param name="node">Node that generates simulated input.</param>
    /// <param name="seconds">Time, in seconds.</param>
    /// <param name="negativeAction">Action name for the negative side of the axis.</param>
    /// <param name="positiveAction">Action name for the positive side of the axis.</param>
    /// <param name="axisPosition">The position of the axis input, from -1 to +1.</param>
    /// <returns>Task that completes when the input finishes.</returns>
    public static async Task HoldControllerDoubleAxisInputFor(this Node node, float seconds, string negativeAction, string positiveAction, float axisPosition)
    {
        node.SetControllerDoubleAxisInput(negativeAction, positiveAction, axisPosition);
        await node.Wait(seconds);
        node.EndControllerDoubleAxisInput(negativeAction, positiveAction);
    }

    /// <summary>
    /// Set a simulated input value for a one-sided analog axis on a controller (for instance, a gamepad trigger).
    /// </summary>
    /// <param name="node">Node that generates simulated input.</param>
    /// <param name="action">Action name for the axis.</param>
    /// <param name="axisPosition">The position of the axis input, from 0 to 1.</param>
    /// <seealso cref="Input.GetActionStrength(StringName, bool)"/>
    public static void SetControllerSingleAxisInput(this Node node, string action, float axisPosition)
    {
        if (axisPosition == 0)
        {
            node.EndAction(action);
        }
        else
        {
            node.StartAction(action, axisPosition);
        }
    }

    /// <summary>
    /// End a simulated input for a one-sided analog axis on a controller (for instance, a gamepad trigger).
    /// </summary>
    /// <param name="node">Node that generates simulated input.</param>
    /// <param name="action">Action name for the axis.</param>
    public static void EndControllerSingleAxisInput(this Node node, string action)
    {
        node.EndAction(action);
    }

    /// <summary>
    /// Hold a one-sided analog-axis controller input for a given duration.
    /// </summary>
    /// <param name="node">Node that generates simulated input.</param>
    /// <param name="seconds">Time, in seconds.</param>
    /// <param name="action">Action name for the axis.</param>
    /// <param name="axisPosition">The position of the axis input, from 0 to 1.</param>
    /// <returns>Task that completes when the input finishes.</returns>
    public static async Task HoldControllerSingleAxisInputFor(this Node node, float seconds, string action, float axisPosition)
    {
        node.SetControllerSingleAxisInput(action, axisPosition);
        await node.Wait(seconds);
        node.EndControllerSingleAxisInput(action);
    }
}
