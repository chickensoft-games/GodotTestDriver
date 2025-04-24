namespace Chickensoft.GodotTestDriver.Drivers;

using System;
using Godot;
using GodotTestDriver.Input;

/// <summary>
/// Driver for a <see cref="GraphNode"/>
/// </summary>
/// <typeparam name="T">GraphNode type.</typeparam>
public class GraphNodeDriver<T> : ControlDriver<T> where T : GraphNode
{
    /// <summary>
    /// Creates a new generic GraphNodeDriver.
    /// </summary>
    /// <param name="producer">Producer that creates a GraphNode subclass.</param>
    /// <param name="description">Driver description.</param>
    public GraphNodeDriver(Func<T> producer, string description = "") : base(producer, description)
    {
    }

    /// <summary>
    /// The title of the node.
    /// </summary>
    public string Title => PresentRoot.Title;

    /// <summary>
    /// The offset of the node inside the graph.
    /// </summary>
    public Vector2 Offset => PresentRoot.PositionOffset;

    /// <summary>
    /// The relative position and size of the node.
    /// </summary>
    public Rect2 Rect => PresentRoot.GetRect();

    /// <summary>
    /// Whether the node is currently selected.
    /// </summary>
    public bool Selected => PresentRoot.Selected;

    /// <summary>
    /// The amount of input ports the node has.
    /// </summary>
    public int InputPortCount => PresentRoot.GetInputPortCount();

    /// <summary>
    /// The amount of output ports the node has.
    /// </summary>
    public int OutputPortCount => PresentRoot.GetOutputPortCount();

    /// <summary>
    /// Returns the port type of the given port.
    /// </summary>
    /// <param name="port">Connection port.</param>
    /// <exception cref="ArgumentException" />
    public int GetPortType(Port port)
    {
        if (!port.IsDefined)
        {
            throw new ArgumentException("Port is not defined.");
        }

        if (port.IsInput)
        {
            if (InputPortCount < port.PortIndex)
            {
                throw new ArgumentException("Port index is out of range.");
            }
            return PresentRoot.GetInputPortType(port.PortIndex);
        }
        if (OutputPortCount < port.PortIndex)
        {
            throw new ArgumentException("Port index is out of range.");
        }

        return PresentRoot.GetOutputPortType(port.PortIndex);
    }

    /// <summary>
    /// Global position of a spot where the node can be safely clicked and dragged. Will fail if the node is not visible.
    /// </summary>
    protected virtual Vector2 SelectionSpot
    {
        get
        {
            var node = VisibleRoot;
            var rect = node.GetGlobalRect();
            // we assume that a position 5 pixels below the top border horizontally centered is a safe selection
            // spot as the node will have a title there.
            return rect.Position + new Vector2(rect.Size.X / 2, 5);
        }
    }

    /// <summary>
    /// Drags the node by the given amount of pixels.
    /// </summary>
    /// <param name="delta">Change in distance.</param>
    public void DragBy(Vector2 delta)
    {
        var dragStart = SelectionSpot;
        var dragEnd = dragStart + delta;

        // drag it
        Viewport.DragMouse(dragStart, dragEnd);
    }

    /// <summary>
    /// Drags the node by the given amount of pixels.
    /// </summary>
    /// <param name="x">Horizontal cartesian coordinate component.</param>
    /// <param name="y">Vertical cartesian coordinate component.</param>
    public void DragBy(float x, float y)
    {
        DragBy(new Vector2(x, y));
    }

    /// <summary>
    /// Drags the node by a multiple of its own size multiplied by the given factor.
    /// </summary>
    /// <param name="delta">Change in distance.</param>
    public void DragByOwnSize(Vector2 delta)
    {
        var node = VisibleRoot;
        var rect = node.GetRect();

        DragBy(new Vector2(rect.Size.X * delta.X, rect.Size.X * delta.Y));
    }

    /// <summary>
    /// Drags the node by a multiple of its own size multiplied by the given factor.
    /// </summary>
    /// <param name="x">Horizontal cartesian coordinate component.</param>
    /// <param name="y">Vertical cartesian coordinate component.</param>
    public void DragByOwnSize(float x, float y)
    {
        DragByOwnSize(new Vector2(x, y));
    }

    /// <summary>
    /// Selects the given node by clicking on it. Same as <see cref="ClickAtSelectionSpot"/>.
    /// </summary>
    public void Select()
    {
        ClickAtSelectionSpot();
    }

    /// <summary>
    /// Clicks the mouse at the safe selection spot of this graph node.
    /// </summary>
    /// <param name="button">Button to use.</param>
    public void ClickAtSelectionSpot(MouseButton button = MouseButton.Left)
    {
        Viewport.ClickMouseAt(SelectionSpot, button);
    }

    /// <summary>
    /// Drags a connection from the given source port of this node to the given target port of the given target node.
    /// </summary>
    /// <param name="sourcePort">Source port.</param>
    /// <param name="targetNode">Target node.</param>
    /// <param name="targetPort">Target node connection port.</param>
    /// <exception cref="ArgumentException"/>
    public void DragConnection(Port sourcePort, GraphNodeDriver<T> targetNode, Port targetPort)
    {
        if (!sourcePort.IsDefined)
        {
            throw new ArgumentException("Source port is not defined.");
        }

        if (!targetPort.IsDefined)
        {
            throw new ArgumentException("Target port is not defined.");
        }

        var thisRoot = VisibleRoot;
        var targetRoot = targetNode.VisibleRoot;

        if (sourcePort.IsInput && sourcePort.PortIndex >= thisRoot.GetInputPortCount())
        {
            throw new ArgumentException($"Node has no input port at the given index {sourcePort.PortIndex}.");
        }

        if (sourcePort.IsOutput && sourcePort.PortIndex >= thisRoot.GetOutputPortCount())
        {
            throw new ArgumentException($"Node has no output port at the given index {sourcePort.PortIndex}.");
        }

        if (targetPort.IsInput && targetPort.PortIndex >= targetRoot.GetInputPortCount())
        {
            throw new ArgumentException(
                $"Target node has no input port at the given index {targetPort.PortIndex}.");
        }

        if (targetPort.IsOutput && targetPort.PortIndex >= targetRoot.GetOutputPortCount())
        {
            throw new ArgumentException(
                $"Target node has no output port at the given index {targetPort.PortIndex}.");
        }

        var startPosition = sourcePort.IsInput
            ? thisRoot.GetInputPortPosition(sourcePort.PortIndex)
            : thisRoot.GetOutputPortPosition(sourcePort.PortIndex);
        var endPosition = targetPort.IsInput
            ? targetRoot.GetInputPortPosition(targetPort.PortIndex)
            : targetRoot.GetOutputPortPosition(targetPort.PortIndex);

        Viewport.DragMouse(startPosition + thisRoot.GlobalPosition,
            endPosition + targetRoot.GlobalPosition);
    }

    /// <summary>
    /// Drags a connection from the given source port of this node to a position relative to this port.
    /// </summary>
    /// <param name="sourcePort">Source port.</param>
    /// <param name="relativePosition">Position offset.</param>
    /// <exception cref="ArgumentException"/>
    public void DragConnection(Port sourcePort, Vector2 relativePosition)
    {
        if (!sourcePort.IsDefined)
        {
            throw new ArgumentException("Source port is not defined.");
        }

        var thisRoot = VisibleRoot;

        if (sourcePort.IsInput && sourcePort.PortIndex >= thisRoot.GetInputPortCount())
        {
            throw new ArgumentException($"Node has no input port at the given index {sourcePort.PortIndex}.");
        }

        if (sourcePort.IsOutput && sourcePort.PortIndex >= thisRoot.GetOutputPortCount())
        {
            throw new ArgumentException($"Node has no output port at the given index {sourcePort.PortIndex}.");
        }

        var startPosition = sourcePort.IsInput
            ? thisRoot.GetInputPortPosition(sourcePort.PortIndex)
            : thisRoot.GetOutputPortPosition(sourcePort.PortIndex);
        var endPosition = startPosition + relativePosition;

        Viewport.DragMouse(startPosition + thisRoot.GlobalPosition,
            endPosition + thisRoot.GlobalPosition);
    }
}

/// <summary>
/// Driver for a <see cref="GraphNode"/>
/// </summary>
public sealed class GraphNodeDriver : GraphNodeDriver<GraphNode>
{
    /// <summary>
    /// Creates a new GraphNodeDriver.
    /// </summary>
    /// <param name="producer">Producer that creates a GraphNode subclass.</param>
    /// <param name="description">Driver description.</param>
    public GraphNodeDriver(Func<GraphNode> producer, string description = "") : base(producer, description)
    {
    }
}
