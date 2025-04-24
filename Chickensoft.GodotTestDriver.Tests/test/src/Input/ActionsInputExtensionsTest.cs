namespace Chickensoft.GodotTestDriver.Tests.Input;

using Chickensoft.GoDotTest;
using Godot;
using GodotTestDriver.Input;
using JetBrains.Annotations;
using Shouldly;

[UsedImplicitly]
public partial class ActionsInputExtensionsTest : InputTest
{
    private partial class ActionInputEventTestNode : Node
    {
        public bool HasInputEventFired { get; set; }
        public bool WasInputPressed { get; set; }
        public StringName InputEventName { get; set; } = string.Empty;

        public override void _Input(InputEvent @event)
        {
            if (@event.IsAction(InputEventName))
            {
                HasInputEventFired = true;
                WasInputPressed = @event.IsActionPressed(InputEventName);
            }
        }
    }

    private const string TestAction = "test_action";

    public ActionsInputExtensionsTest(Node testScene) : base(testScene)
    {
    }

    [Test]
    public void StartActionSetsGlobalActionPressed()
    {
        Input.IsActionPressed(TestAction).ShouldBeFalse();
        RootNode.StartAction(TestAction);
        Input.IsActionPressed(TestAction).ShouldBeTrue();
        RootNode.EndAction(TestAction);
    }

    [Test]
    public void StartActionClampsStrengthBetweenZeroAndOne()
    {
        RootNode.StartAction(TestAction, -1);
        Input.GetActionStrength(TestAction).ShouldBe(0);
        RootNode.EndAction(TestAction);
        RootNode.StartAction(TestAction, 2);
        Input.GetActionStrength(TestAction).ShouldBe(1);
        RootNode.EndAction(TestAction);
    }

    [Test]
    public void EndActionRemovesGlobalActionPressed()
    {
        RootNode.StartAction(TestAction);
        RootNode.EndAction(TestAction);
        Input.IsActionPressed(TestAction).ShouldBeFalse();
    }

    [Test]
    public void StartActionSetsGlobalActionJustPressed()
    {
        RootNode.StartAction(TestAction);
        Input.IsActionJustPressed(TestAction).ShouldBeTrue();
        RootNode.EndAction(TestAction);
    }

    [Test]
    public void EndActionSetsGlobalActionJustReleased()
    {
        RootNode.StartAction(TestAction);
        RootNode.EndAction(TestAction);
        Input.IsActionJustReleased(TestAction).ShouldBeTrue();
    }

    [Test]
    public void StartActionSendsInputEvent()
    {
        var inputTestNode = new ActionInputEventTestNode
        {
            InputEventName = TestAction
        };
        RootNode.AddChild(inputTestNode);
        inputTestNode.HasInputEventFired.ShouldBeFalse();
        inputTestNode.StartAction(TestAction);
        inputTestNode.HasInputEventFired.ShouldBeTrue();
        inputTestNode.WasInputPressed.ShouldBeTrue();
        inputTestNode.EndAction(TestAction);
        RootNode.RemoveChild(inputTestNode); // Remove immediately since we won't wait a frame for the free
        inputTestNode.QueueFree();
    }

    [Test]
    public void EndActionSendsInputEvent()
    {
        var inputTestNode = new ActionInputEventTestNode
        {
            InputEventName = TestAction
        };
        RootNode.AddChild(inputTestNode);
        inputTestNode.HasInputEventFired.ShouldBeFalse();
        inputTestNode.EndAction(TestAction);
        inputTestNode.HasInputEventFired.ShouldBeTrue();
        inputTestNode.WasInputPressed.ShouldBeFalse();
        RootNode.RemoveChild(inputTestNode); // Remove immediately since we won't wait a frame for the free
        inputTestNode.QueueFree();
    }
}
