namespace Chickensoft.GodotTestDriver.Drivers;

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using JetBrains.Annotations;
using Object = Godot.GodotObject;

/// <summary>
/// Base class for test drivers that work on nodes. This is the non-generic variant, which will make it easier to
/// create some fluent APIs.
/// </summary>
public abstract class NodeDriver
{
}

/// <summary>
/// Base class for test drivers that work on nodes.
/// </summary>
/// <typeparam name="T">Node type.</typeparam>
[PublicAPI]
public abstract class NodeDriver<T> : NodeDriver where T : Node
{
    private readonly Func<T> _producer;

    /// <summary>
    /// The description given to the test driver.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Creates a new generic NodeDriver.
    /// </summary>
    /// <param name="producer">Producer that creates a Node subclass.</param>
    /// <param name="description">Driver description.</param>
    protected NodeDriver(Func<T> producer, string description = "")
    {
        _producer = producer;
        Description = description;
    }

    /// <summary>
    /// Is the node currently present in the tree?
    /// </summary>
    public bool IsPresent => Root != null;

    /// <summary>
    /// Builds drivers for a set of children of the current driver's root node. The first function needs to return
    /// the currently applicable child nodes, the second function will produce a driver for each child.
    /// </summary>
    /// <typeparam name="TDriver">Driver type.</typeparam>
    /// <typeparam name="TNode">Node type.</typeparam>
    /// <param name="childSelector">Child selector.</param>
    /// <param name="driverFactory">Driver factory.</param>
    protected IEnumerable<TDriver> BuildDrivers<TDriver, TNode>(Func<T, IEnumerable<TNode>> childSelector,
        Func<Func<TNode>, TDriver> driverFactory)
        where TDriver : NodeDriver where TNode : Node
    {
        var root = Root;
        if (root == null)
        {
            yield break;
        }

        foreach (var child in childSelector(root).ToList())
        {
            yield return driverFactory(() => root.GetNode<TNode>(child.GetPath()));
        }
    }

    /// <summary>
    /// Helper function to build an error message. Prefixes the message with a human readable description of this
    /// driver.
    /// </summary>
    /// <param name="message">Error message.</param>
    protected string ErrorMessage(string message)
    {
        // if description is blank or empty, use the type name as description
        var typeName = GetType().Name;
        if (string.IsNullOrEmpty(Description))
        {
            return $"{typeName}: {message}";
        }

        return $"{typeName} [{Description}] {message}";
    }

    /// <summary>
    /// Returns the root node. Can be null in case the root
    /// node is not currently present in the scene tree or if the root node is not a valid instance anymore.
    /// </summary>
    [CanBeNull]
    public T? Root
    {
        get
        {
            var node = _producer();
            return Object.IsInstanceValid(node) && node.IsInsideTree() ? node : null;
        }
    }

    /// <summary>
    /// Returns the root node and ensures it is present.
    /// </summary>
    /// <exception cref="InvalidOperationException"/>
    public T PresentRoot => Root ?? throw new InvalidOperationException(ErrorMessage("Node is not present in the scene tree."));

    /// <summary>
    /// Returns whether this node has the given signal connected to the given target.
    /// </summary>
    /// <param name="signal">Signal name.</param>
    /// <param name="target">Target object.</param>
    /// <param name="method">Method name.</param>
    public bool IsSignalConnected(string signal, Object target, string method)
    {
        return PresentRoot.IsConnected(signal, new Callable(target, method));
    }

    /// <summary>
    /// Returns whether the given signal of this node is connected to any other node.
    /// </summary>
    /// <param name="signal">Signal name.</param>
    public bool IsSignalConnectedToAnywhere(string signal)
    {
        return PresentRoot.GetSignalConnectionList(signal).Count > 0;
    }

    /// <summary>
    /// Returns a signal awaiter that can be used to wait for the given signal.
    /// </summary>
    /// <param name="signal">Signal name.</param>
    public SignalAwaiter GetSignalAwaiter(StringName signal)
    {
        return PresentRoot.ToSignal(PresentRoot, signal);
    }
}
