namespace Chickensoft.GodotTestDriver.Util;

using System;
using System.Threading.Tasks;
using Godot;

/// <summary>
/// Wait extensions for Godot nodes and scene trees.
/// </summary>
public static class WaitExtensions
{
    /// <summary>
    /// <para>
    /// Waits for the given amount of time that the given action performs
    /// without any exception. If the action throws an exception it will be
    /// repeated until it no longer throws an exception or the timeout is
    /// reached. When the timeout is reached the last exception will be
    /// thrown.
    /// </para>
    /// <para>
    /// This can be used to wait for an assertion to become true within a
    /// certain period of time.
    /// </para>
    /// </summary>
    /// <param name="tree">Scene tree.</param>
    /// <param name="seconds">Time to wait.</param>
    /// <param name="action">Action to perform.</param>
    public static async Task WithinSeconds(this SceneTree tree, float seconds, Action action)
    {
        var timeout = new Timeout(seconds);
        while (true)
        {
            await tree.NextFrame();
            try
            {
                action();
                return;
            }
            catch (Exception) when (timeout.IsReached)
            {
                throw;
            }
        }
    }

    /// <summary>
    /// <para>
    /// Waits for the given amount of time that the given action performs
    /// without any exception. If the action throws an exception it will be
    /// repeated until it no longer throws an exception or the timeout is
    /// reached. When the timeout is reached the last exception will be
    /// thrown.
    /// </para>
    /// <para>
    /// This can be used to wait for an assertion to become true within a
    /// certain period of time.
    /// </para>
    /// </summary>
    /// <param name="node">Godot node.</param>
    /// <param name="seconds">Time to wait.</param>
    /// <param name="action">Action to perform.</param>
    public static async Task WithinSeconds(this Node node, float seconds, Action action)
    {
        await node.VerifyInTree().WithinSeconds(seconds, action);
    }

    /// <summary>
    /// Waits for the given amount of seconds for the given condition to be
    /// true. If the condition is not true after the timeout is reached, an
    /// exception will be thrown.
    /// </summary>
    /// <param name="tree">Scene tree.</param>
    /// <param name="seconds">Time to wait.</param>
    /// <param name="condition">Predicate that will be invoked after
    /// waiting.</param>
    /// <exception cref="TimeoutException" />
    public static async Task WithinSeconds(this SceneTree tree, float seconds, Func<bool> condition)
    {
        var timeout = new Timeout(seconds);
        do
        {
            await tree.NextFrame();
            if (condition())
            {
                return;
            }
        } while (!timeout.IsReached);

        throw new TimeoutException("Condition was not true within the given time.");
    }

    /// <summary>
    /// Waits for the given amount of seconds for the given condition to be
    /// true. If the condition is not true after the timeout is reached, an
    /// exception will be thrown.
    /// </summary>
    /// <param name="node">Godot node.</param>
    /// <param name="seconds">Time to wait.</param>
    /// <param name="condition">Predicate that will be invoked after
    /// waiting.</param>
    /// <exception cref="TimeoutException" />
    public static async Task WithinSeconds(this Node node, float seconds, Func<bool> condition)
    {
        await node.VerifyInTree().WithinSeconds(seconds, condition);
    }

    /// <summary>
    /// Runs the given action repeatedly every frame for the given amount
    /// of seconds. If the action throws an exception, the exception will
    /// be thrown and the loop will be stopped. This can be used to check
    /// an assertion to be true for a certain period of time.
    /// </summary>
    /// <param name="tree">Scene tree.</param>
    /// <param name="seconds">Time to wait.</param>
    /// <param name="action">Action to perform each frame.</param>
    public static async Task DuringSeconds(this SceneTree tree, float seconds, Action action)
    {
        var timeout = new Timeout(seconds);
        do
        {
            action();
            await tree.NextFrame();
        } while (!timeout.IsReached);
    }

    /// <summary>
    /// Runs the given action repeatedly every frame for the given amount
    /// of seconds. If the action throws an exception, the exception will
    /// be thrown and the loop will be stopped. This can be used to check
    /// an assertion to be true for a certain period of time.
    /// </summary>
    /// <param name="node">Godot node.</param>
    /// <param name="seconds">Time to wait.</param>
    /// <param name="action">Action to perform each frame.</param>
    public static async Task DuringSeconds(this Node node, float seconds, Action action)
    {
        await node.VerifyInTree().DuringSeconds(seconds, action);
    }

    /// <summary>
    /// Waits for the events triggered by the most recent action to be
    /// processed by waiting for 2 frames.
    /// </summary>
    /// <param name="tree">Scene tree.</param>
    public static async Task WaitForEvents(this SceneTree tree)
    {
        await tree.NextFrame(2);
    }

    /// <summary>
    /// Waits for the events triggered by the most recent action to be
    /// processed by waiting for 2 frames.
    /// </summary>
    /// <param name="node">Godot node.</param>
    public static async Task WaitForEvents(this Node node)
    {
        await node.VerifyInTree().WaitForEvents();
    }

    /// <summary>
    /// Waits until the given amount of frames have passed. Returns in the
    /// context of the `Process` method on the main thread.
    /// </summary>
    /// <param name="tree">Scene tree.</param>
    /// <param name="frames">Optional number of frames to wait, defaulting
    /// to a single frame.</param>
    public static async Task NextFrame(this SceneTree tree, int frames = 1)
    {
        while (frames > 0)
        {
            await tree.ToSignal(tree, SceneTree.SignalName.ProcessFrame);
            frames--;
        }
    }

    /// <summary>
    /// Waits until the given number of frames have passed. Returns in the
    /// context of the `Process` method on the main thread.
    /// </summary>
    /// <param name="node">Godot node.</param>
    /// <param name="frames">Optional number of frames to wait, defaulting
    /// to a single frame.</param>
    /// <exception cref="InvalidOperationException" />
    public static async Task ProcessFrame(this Node node, int frames = 1)
    {
        await node.VerifyInTree().NextFrame(frames);
    }

    /// <summary>
    /// Waits until the given number of physics frames have passed. Returns
    /// in the context of the `PhysicsProcess` method on the main thread.
    /// </summary>
    /// <param name="tree">Scene tree.</param>
    /// <param name="frames">Number of frames to wait.</param>
    public static async Task PhysicsProcessFrame(this SceneTree tree, int frames = 1)
    {
        while (frames > 0)
        {
            await tree.ToSignal(tree, SceneTree.SignalName.PhysicsFrame);
            frames--;
        }
    }

    /// <summary>
    /// Waits until the given number of physics frames have passed. Returns
    /// in the context of the `PhysicsProcess` method on the main thread.
    /// </summary>
    /// <param name="node">Godot node.</param>
    /// <param name="frames">Number of frames to wait.</param>
    public static async Task PhysicsProcessFrame(this Node node, int frames = 1)
    {
        await node.VerifyInTree().PhysicsProcessFrame(frames);
    }

    /// <summary>
    /// Waits for frames to pass until the given amount of time have
    /// elapsed.
    /// </summary>
    /// <param name="node">Godot node.</param>
    /// <param name="seconds">Time to wait.</param>
    public static async Task Wait(this Node node, float seconds)
    {
        await node.VerifyInTree().Wait(seconds);
    }

    /// <summary>
    /// Waits for frames to pass until the given amount of time has
    /// elapsed.
    /// </summary>
    /// <param name="tree">Scene tree.</param>
    /// <param name="seconds">Time to wait.</param>
    public static async Task Wait(this SceneTree tree, float seconds)
    {
        var timeout = new Timeout(seconds);
        while (!timeout.IsReached)
        {
            await tree.NextFrame();
        }
    }

    /// <summary>
    /// Waits for physics frames to pass until the given amount of time has
    /// elapsed.
    /// </summary>
    /// <param name="node">Godot node.</param>
    /// <param name="seconds">Time to wait.</param>
    public static async Task WaitPhysics(this Node node, float seconds)
    {
        await node.VerifyInTree().WaitPhysics(seconds);
    }

    /// <summary>
    /// Waits for physics frames to pass until the given amount of time has
    /// elapsed.
    /// </summary>
    /// <param name="tree">Scene tree.</param>
    /// <param name="seconds">Time to wait.</param>
    public static async Task WaitPhysics(this SceneTree tree, float seconds)
    {
        var timeout = new Timeout(seconds);
        while (!timeout.IsReached)
        {
            await tree.PhysicsProcessFrame();
        }
    }

    /// <summary>
    /// Waits until the given predicate returns true, or the timeout
    /// occurs. The predicate is invoked in the context of the `Process`
    /// method on the main thread.
    /// </summary>
    /// <param name="node">Godot node.</param>
    /// <param name="condition">Predicate to check each frame.</param>
    /// <param name="timeoutSeconds">Time to wait.</param>
    public static async Task WaitUntil(this Node node, Func<bool> condition, float timeoutSeconds = 3.0f)
    {
        await node.VerifyInTree().WaitUntil(condition, timeoutSeconds);
    }

    /// <summary>
    /// Waits until the given predicate returns true, or the timeout
    /// occurs. The predicate is invoked in the context of the `Process`
    /// method on the main thread.
    /// </summary>
    /// <param name="tree">Scene tree.</param>
    /// <param name="condition">Predicate to check each frame.</param>
    /// <param name="timeoutSeconds">Time to wait.</param>
    /// <exception cref="TimeoutException" />
    public static async Task WaitUntil(this SceneTree tree, Func<bool> condition, float timeoutSeconds = 3.0f)
    {
        var timeout = new Timeout(timeoutSeconds);
        while (!condition() && !timeout.IsReached)
        {
            await tree.NextFrame();
        }

        if (!condition())
        {
            throw new TimeoutException("Timeout while waiting for condition to be true.");
        }
    }

    /// <summary>
    /// Verify that the given node is inside a tree, and return the tree. Throws an InvalidOperationException
    /// if the node is not inside a tree.
    /// </summary>
    /// <param name="node">Node to verify.</param>
    /// <exception cref="InvalidOperationException"></exception>
    private static SceneTree VerifyInTree(this Node node)
    {
        return !node.IsInsideTree()
            ? throw new InvalidOperationException("Node is not inside a tree.")
            : node.GetTree();
    }
}
