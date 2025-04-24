namespace Chickensoft.GodotTestDriver.Drivers;

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using JetBrains.Annotations;

/// <summary>
/// Driver for a <see cref="GraphEdit"/>.
/// </summary>
/// <typeparam name="TGraphEdit">GraphEdit type.</typeparam>
/// <typeparam name="TGraphNodeDriver">GraphNodeDriver type.</typeparam>
/// <typeparam name="TGraphNode">GraphNode type.</typeparam>
[PublicAPI]
public class GraphEditDriver<TGraphEdit, TGraphNodeDriver, TGraphNode> : ControlDriver<TGraphEdit>
    where TGraphEdit : GraphEdit where TGraphNode : GraphNode where TGraphNodeDriver : GraphNodeDriver<TGraphNode>
{
    private readonly Func<Func<TGraphNode>, string, TGraphNodeDriver> _nodeDriverProducer;

    /// <summary>
    /// Constructs a new GraphEdit driver.
    /// </summary>
    /// <param name="producer">a producer that produces the <see cref="GraphEdit"/> that this driver works on.</param>
    /// <param name="nodeDriverProducer">a producer that produces a driver for a <see cref="GraphNode"/> child of the <see cref="GraphEdit"/>.</param>
    /// <param name="description">a description for the node.</param>
    public GraphEditDriver(Func<TGraphEdit> producer,
        Func<Func<TGraphNode>, string, TGraphNodeDriver> nodeDriverProducer,
        string description = "") : base(producer, description)
    {
        _nodeDriverProducer = nodeDriverProducer;
    }

    /// <summary>
    /// Checks if the graph edit has a connection from the given node to the given target node on the
    /// given ports.
    /// </summary>
    /// <param name="from">Source GraphNodeDriver.</param>
    /// <param name="fromPort">Source port.</param>
    /// <param name="to">Target GraphNodeDriver.</param>
    /// <param name="toPort">Target port.</param>
    /// <exception cref="ArgumentException" />
    public bool HasConnection(TGraphNodeDriver from, Port fromPort, TGraphNodeDriver to, Port toPort)
    {
        if (!fromPort.IsOutput)
        {
            throw new ArgumentException("fromPort must be an output port");
        }

        if (!toPort.IsInput)
        {
            throw new ArgumentException("toPort must be an input port");
        }

        var graphEdit = PresentRoot;
        var fromRoot = from.PresentRoot;
        var toRoot = to.PresentRoot;

        return graphEdit.GetConnectionList()
            .Any(connection =>
                (string)connection["from_node"] == fromRoot.Name
                && (int)connection["from_port"] == fromPort.PortIndex
                && (string)connection["to_node"] == toRoot.Name
                && (int)connection["to_port"] == toPort.PortIndex);
    }

    /// <summary>
    /// Checks if he graph edit has a connection originating from the given node on the given port.
    /// </summary>
    /// <param name="from">Source GraphNodeDriver.</param>
    /// <param name="fromPort">Source port.</param>
    /// <exception cref="ArgumentException"/>
    public bool HasConnectionFrom(TGraphNodeDriver from, Port fromPort)
    {
        if (!fromPort.IsOutput)
        {
            throw new ArgumentException("fromPort must be an output port");
        }

        var graphEdit = PresentRoot;
        var fromRoot = from.PresentRoot;

        return graphEdit.GetConnectionList().Cast<Dictionary>()
            .Any(connection =>
                (string)connection["from_node"] == fromRoot.Name
                && (int)connection["from_port"] == fromPort.PortIndex
            );
    }

    /// <summary>
    /// Checks if he graph edit has a connection originating from the given node on the given port.
    /// </summary>
    /// <param name="to">Target GraphNodeDriver.</param>
    /// <param name="toPort">Target port.</param>
    /// <exception cref="ArgumentException"/>
    public bool HasConnectionTo(TGraphNodeDriver to, Port toPort)
    {
        if (!toPort.IsInput)
        {
            throw new ArgumentException("toPort must be an input port");
        }

        var graphEdit = PresentRoot;
        var toRoot = to.PresentRoot;

        return graphEdit.GetConnectionList().Cast<Dictionary>()
            .Any(connection =>
                (string)connection["to_node"] == toRoot.Name
                && (int)connection["to_port"] == toPort.PortIndex
            );
    }

    /// <summary>
    /// Drivers corresponding to the nodes in the graph.
    /// </summary>
    public IEnumerable<TGraphNodeDriver> Nodes =>
        BuildDrivers(root => root.GetChildren().OfType<TGraphNode>(),
            node => _nodeDriverProducer(node, "-> GraphNode")
        );
}

/// <summary>
/// Driver for a <see cref="GraphEdit"/>.
/// </summary>
[PublicAPI]
public class GraphEditDriver : GraphEditDriver<GraphEdit, GraphNodeDriver, GraphNode>
{
    /// <summary>
    /// Constructs a new GraphEdit driver.
    /// </summary>
    /// <param name="producer">a producer that produces the <see cref="GraphEdit"/> that this driver works on.</param>
    /// <param name="description">Driver description.</param>
    public GraphEditDriver(Func<GraphEdit> producer, string description = "") : base(producer,
        (node, nodeDescription) => new GraphNodeDriver(node, $"{description}-> {nodeDescription}"),
        description)
    {
    }
}
