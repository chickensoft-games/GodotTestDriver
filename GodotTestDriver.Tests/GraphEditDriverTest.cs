namespace Chickensoft.GodotTestDriver.Tests;

using System.Linq;
using System.Threading.Tasks;
using Chickensoft.GoDotTest;
using Godot;
using GodotTestDriver.Drivers;
using JetBrains.Annotations;
using Shouldly;

[UsedImplicitly]
public class GraphEditDriverTest : DriverTest
{
    private readonly GraphEditDriver _graphEdit;

    public GraphEditDriverTest(Node testScene) : base(testScene)
    {
        _graphEdit = new GraphEditDriver(() => RootNode.GetNode<GraphEdit>("GraphEdit"));
    }

    [Test]
    public void InspectionWorks()
    {
        // WHEN
        // everything is set up

        // THEN
        // the graph edit has two nodes
        _graphEdit.Nodes.Count().ShouldBe(2);

        var firstNode = _graphEdit.Nodes.First();
        var secondNode = _graphEdit.Nodes.Last();

        // both nodes are fully in view
        firstNode.IsFullyInView.ShouldBeTrue();
        secondNode.IsFullyInView.ShouldBeTrue();

        // the first node has no connection
        _graphEdit.HasConnectionFrom(firstNode, Port.Output(0)).ShouldBeFalse();

        // the second node has no connection
        _graphEdit.HasConnectionFrom(secondNode, Port.Output(0)).ShouldBeFalse();

        // the first node has an output port but no input port
        firstNode.OutputPortCount.ShouldBe(1);
        firstNode.InputPortCount.ShouldBe(0);

        // the second node has an input port but no output port
        secondNode.OutputPortCount.ShouldBe(0);
        secondNode.InputPortCount.ShouldBe(1);
    }

    [Test]
    public async Task DraggingNodesWorks()
    {
        // SETUP
        var firstNode = _graphEdit.Nodes.First();

        var firstNodeOffset = firstNode.Offset;
        // WHEN
        // i drag the first node
        await firstNode.DragByOwnSize(2, 0);

        // THEN
        // the offset has changed by 2x it's width
        // (pixels seem to be doubled on macOS :P)
        var scale = (OS.GetName() == "macOS") ? 2f : 1f;
        (firstNode.Offset.X * scale).ShouldBe(
            firstNodeOffset.X + (firstNode.Rect.Size.X * 2), 10
        );
    }

    [Test]
    public async Task DraggingConnectionsWorks()
    {
        // SETUP
        var firstNode = _graphEdit.Nodes.First();
        var secondNode = _graphEdit.Nodes.Last();

        // WHEN
        // i drag a connection from the first node to the second node
        await firstNode.DragConnection(Port.Output(0), secondNode, Port.Input(0));

        // THEN
        // the connection works
        _graphEdit.HasConnection(firstNode, Port.Output(0), secondNode, Port.Input(0)).ShouldBeTrue();
    }

    [Test]
    public async Task DraggingConnectionsInReverseWorks()
    {
        // SETUP
        var firstNode = _graphEdit.Nodes.First();
        var secondNode = _graphEdit.Nodes.Last();

        // WHEN
        // i drag a connection from the second node to the first node
        await secondNode.DragConnection(Port.Input(0), firstNode, Port.Output(0));

        // THEN
        // the connection works
        _graphEdit.HasConnection(firstNode, Port.Output(0), secondNode, Port.Input(0)).ShouldBeTrue();
    }

    [Test]
    public async Task DisconnectingWorks()
    {
        // SETUP
        var firstNode = _graphEdit.Nodes.First();
        var secondNode = _graphEdit.Nodes.Last();

        // make a connection
        await firstNode.DragConnection(Port.Output(0), secondNode, Port.Input(0));

        // WHEN
        // i disconnect the connection, by dragging it from the second node to the first node
        await secondNode.DragConnection(Port.Input(0), firstNode, Port.Output(0));

        // THEN
        // the connection is gone
        _graphEdit.HasConnection(firstNode, Port.Output(0), secondNode, Port.Input(0)).ShouldBeFalse();
    }
}
