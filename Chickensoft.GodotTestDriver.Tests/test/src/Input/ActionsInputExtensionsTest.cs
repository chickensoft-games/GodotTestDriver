namespace Chickensoft.GodotTestDriver.Tests.Input;

using Chickensoft.GoDotTest;
using Godot;
using GodotTestDriver.Input;
using Shouldly;

public partial class ActionsInputExtensionsTest : InputTest
{
  private sealed partial class ActionInputEventTestNode : Node
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

  private const string TEST_ACTION = "test_action";

  public ActionsInputExtensionsTest(Node testScene) : base(testScene)
  {
  }

  [Test]
  public void StartActionSetsGlobalActionPressed()
  {
    Input.IsActionPressed(TEST_ACTION).ShouldBeFalse();
    RootNode.StartAction(TEST_ACTION);
    Input.IsActionPressed(TEST_ACTION).ShouldBeTrue();
    RootNode.EndAction(TEST_ACTION);
  }

  [Test]
  public void StartActionClampsStrengthBetweenZeroAndOne()
  {
    RootNode.StartAction(TEST_ACTION, -1);
    Input.GetActionStrength(TEST_ACTION).ShouldBe(0);
    RootNode.EndAction(TEST_ACTION);
    RootNode.StartAction(TEST_ACTION, 2);
    Input.GetActionStrength(TEST_ACTION).ShouldBe(1);
    RootNode.EndAction(TEST_ACTION);
  }

  [Test]
  public void EndActionRemovesGlobalActionPressed()
  {
    RootNode.StartAction(TEST_ACTION);
    RootNode.EndAction(TEST_ACTION);
    Input.IsActionPressed(TEST_ACTION).ShouldBeFalse();
  }

  [Test]
  public void StartActionSetsGlobalActionJustPressed()
  {
    RootNode.StartAction(TEST_ACTION);
    Input.IsActionJustPressed(TEST_ACTION).ShouldBeTrue();
    RootNode.EndAction(TEST_ACTION);
  }

  [Test]
  public void EndActionSetsGlobalActionJustReleased()
  {
    RootNode.StartAction(TEST_ACTION);
    RootNode.EndAction(TEST_ACTION);
    Input.IsActionJustReleased(TEST_ACTION).ShouldBeTrue();
  }

  [Test]
  public void StartActionSendsInputEvent()
  {
    var inputTestNode = new ActionInputEventTestNode
    {
      InputEventName = TEST_ACTION
    };
    RootNode.AddChild(inputTestNode);
    inputTestNode.HasInputEventFired.ShouldBeFalse();
    inputTestNode.StartAction(TEST_ACTION);
    inputTestNode.HasInputEventFired.ShouldBeTrue();
    inputTestNode.WasInputPressed.ShouldBeTrue();
    inputTestNode.EndAction(TEST_ACTION);
    // Remove immediately since we won't wait a frame for the free
    RootNode.RemoveChild(inputTestNode);
    inputTestNode.QueueFree();
  }

  [Test]
  public void EndActionSendsInputEvent()
  {
    var inputTestNode = new ActionInputEventTestNode
    {
      InputEventName = TEST_ACTION
    };
    RootNode.AddChild(inputTestNode);
    inputTestNode.HasInputEventFired.ShouldBeFalse();
    inputTestNode.EndAction(TEST_ACTION);
    inputTestNode.HasInputEventFired.ShouldBeTrue();
    inputTestNode.WasInputPressed.ShouldBeFalse();
    // Remove immediately since we won't wait a frame for the free
    RootNode.RemoveChild(inputTestNode);
    inputTestNode.QueueFree();
  }
}
