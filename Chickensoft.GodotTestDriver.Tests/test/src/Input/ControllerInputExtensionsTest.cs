namespace Chickensoft.GodotTestDriver.Tests.Input;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chickensoft.GoDotTest;
using Chickensoft.GodotTestDriver.Input;
using Godot;
using Shouldly;

public partial class ControllerInputExtensionsTest : InputTest
{
  private sealed class TimedButtonEvent
  {
    public ulong ProcessFrame { get; set; }
    public DateTime DateTime { get; set; }
    public InputEventJoypadButton Event { get; set; }

    public TimedButtonEvent(ulong processFrame, DateTime dateTime, InputEventJoypadButton @event)
    {
      ProcessFrame = processFrame;
      DateTime = dateTime;
      Event = @event;
    }
  }

  private sealed partial class JoypadButtonInputEventTestNode : Node
  {
    public IList<TimedButtonEvent> Events { get; } = [];

    public override void _Input(InputEvent @event)
    {
      if (@event is InputEventJoypadButton buttonEvent)
      {
        var frame = Engine.GetProcessFrames();
        var time = DateTime.Now;
        Events.Add(new TimedButtonEvent(frame, time, buttonEvent));
      }
    }
  }

  private sealed class TimedMotionEvent
  {
    public DateTime DateTime { get; set; }
    public InputEventJoypadMotion Event { get; set; }

    public TimedMotionEvent(DateTime dateTime, InputEventJoypadMotion @event)
    {
      DateTime = dateTime;
      Event = @event;
    }
  }

  private sealed partial class JoypadMotionInputEventTestNode : Node
  {
    public IList<TimedMotionEvent> Events { get; } = [];

    public override void _Input(InputEvent @event)
    {
      if (@event is InputEventJoypadMotion buttonEvent)
      {
        var time = DateTime.Now;
        Events.Add(new TimedMotionEvent(time, buttonEvent));
      }
    }
  }

  public ControllerInputExtensionsTest(Node testScene) : base(testScene)
  {
  }

  [Test]
  public void PressJoypadButtonFiresInputEvent()
  {
    var testNode = new JoypadButtonInputEventTestNode();
    RootNode.AddChild(testNode);
    testNode.Events.Count.ShouldBe(0);
    // Press controller device 1's X button.
    var button = JoyButton.X;
    var deviceID = 1;
    testNode.PressJoypadButton(button, deviceID);
    testNode.Events.Count.ShouldBe(1);
    testNode.Events[0].Event.Pressed.ShouldBeTrue();
    testNode.Events[0].Event.ButtonIndex.ShouldBe(button);
    testNode.Events[0].Event.Device.ShouldBe(deviceID);
    // Remove immediately since we won't wait a frame for the free
    RootNode.RemoveChild(testNode);
    testNode.QueueFree();
  }

  [Test]
  public void ReleaseJoypadButtonFiresInputEvent()
  {
    var testNode = new JoypadButtonInputEventTestNode();
    RootNode.AddChild(testNode);
    testNode.Events.Count.ShouldBe(0);
    // Release controller device 1's X button.
    var button = JoyButton.X;
    var deviceID = 1;
    testNode.ReleaseJoypadButton(button, deviceID);
    testNode.Events.Count.ShouldBe(1);
    testNode.Events[0].Event.Pressed.ShouldBeFalse();
    testNode.Events[0].Event.ButtonIndex.ShouldBe(button);
    testNode.Events[0].Event.Device.ShouldBe(deviceID);
    // Remove immediately since we won't wait a frame for the free
    RootNode.RemoveChild(testNode);
    testNode.QueueFree();
  }

  [Test]
  public void TapJoypadButtonFiresInputEvents()
  {
    var testNode = new JoypadButtonInputEventTestNode();
    RootNode.AddChild(testNode);
    testNode.Events.Count.ShouldBe(0);
    // Tap controller device 1's X button.
    var button = JoyButton.X;
    var deviceID = 1;
    testNode.TapJoypadButton(button, deviceID);
    testNode.Events.Count.ShouldBe(2);
    testNode.Events[0].Event.Pressed.ShouldBeTrue();
    testNode.Events[0].Event.ButtonIndex.ShouldBe(button);
    testNode.Events[0].Event.Device.ShouldBe(deviceID);
    testNode.Events[1].Event.Pressed.ShouldBeFalse();
    testNode.Events[1].Event.ButtonIndex.ShouldBe(button);
    testNode.Events[1].Event.Device.ShouldBe(deviceID);
    testNode.Events[1].ProcessFrame.ShouldBe(testNode.Events[0].ProcessFrame);
    // Remove immediately since we won't wait a frame for the free
    RootNode.RemoveChild(testNode);
    testNode.QueueFree();
  }

  [Test]
  public async Task HoldJoypadButtonFiresInputEvents()
  {
    var testNode = new JoypadButtonInputEventTestNode();
    RootNode.AddChild(testNode);
    testNode.Events.Count.ShouldBe(0);
    // Hold controller device 1's X button for 2 seconds.
    var button = JoyButton.X;
    var deviceID = 1;
    var seconds = 0.5f;
    var timeTolerance = 0.1f;
    await testNode.HoldJoypadButtonFor(seconds, button, deviceID);
    testNode.Events.Count.ShouldBe(2);
    testNode.Events[0].Event.Pressed.ShouldBeTrue();
    testNode.Events[0].Event.ButtonIndex.ShouldBe(button);
    testNode.Events[0].Event.Device.ShouldBe(deviceID);
    testNode.Events[1].Event.Pressed.ShouldBeFalse();
    testNode.Events[1].Event.ButtonIndex.ShouldBe(button);
    testNode.Events[1].Event.Device.ShouldBe(deviceID);
    var timeDiff = testNode.Events[1].DateTime - testNode.Events[0].DateTime;
    timeDiff.TotalSeconds.ShouldBe(seconds, timeTolerance);
    // Remove immediately since we won't wait a frame for the free
    RootNode.RemoveChild(testNode);
    testNode.QueueFree();
  }

  [Test]
  public void MoveJoypadAxisFiresInputEvent()
  {
    var testNode = new JoypadMotionInputEventTestNode();
    RootNode.AddChild(testNode);
    testNode.Events.Count.ShouldBe(0);
    // Move controller device 1's right-thumbstick x-axis to the -0.3 position
    // (about 1/3 left stick).
    var axis = JoyAxis.RightX;
    var deviceID = 1;
    var position = -0.3f;
    testNode.MoveJoypadAxisTo(axis, position, deviceID);
    testNode.Events.Count.ShouldBe(1);
    testNode.Events[0].Event.Axis.ShouldBe(axis);
    testNode.Events[0].Event.AxisValue.ShouldBe(position);
    testNode.Events[0].Event.Device.ShouldBe(deviceID);
    RootNode.RemoveChild(testNode);
    testNode.QueueFree();
  }

  [Test]
  public void ReleaseJoypadAxisFiresInputEvent()
  {
    var testNode = new JoypadMotionInputEventTestNode();
    RootNode.AddChild(testNode);
    testNode.Events.Count.ShouldBe(0);
    // Move controller device 1's right-thumbstick x-axis to the rest position.
    var axis = JoyAxis.RightX;
    var deviceID = 1;
    testNode.ReleaseJoypadAxis(axis, deviceID);
    testNode.Events.Count.ShouldBe(1);
    testNode.Events[0].Event.Axis.ShouldBe(axis);
    testNode.Events[0].Event.AxisValue.ShouldBe(0.0f);
    testNode.Events[0].Event.Device.ShouldBe(deviceID);
    RootNode.RemoveChild(testNode);
    testNode.QueueFree();
  }

  [Test]
  public async Task HoldJoypadAxisFiresInputEvents()
  {
    var testNode = new JoypadMotionInputEventTestNode();
    RootNode.AddChild(testNode);
    testNode.Events.Count.ShouldBe(0);
    // Move controller device 1's right-thumbstick x-axis to the rest position.
    var axis = JoyAxis.RightX;
    var deviceID = 1;
    var position = -0.3f;
    var seconds = 0.5f;
    var timeTolerance = 0.1f;
    await testNode.HoldJoypadAxisFor(seconds, axis, position, deviceID);
    testNode.Events.Count.ShouldBe(2);
    testNode.Events[0].Event.Axis.ShouldBe(axis);
    testNode.Events[0].Event.AxisValue.ShouldBe(position);
    testNode.Events[0].Event.Device.ShouldBe(deviceID);
    testNode.Events[1].Event.Axis.ShouldBe(axis);
    testNode.Events[1].Event.AxisValue.ShouldBe(0.0f);
    testNode.Events[1].Event.Device.ShouldBe(deviceID);
    var timeDiff = testNode.Events[1].DateTime - testNode.Events[0].DateTime;
    timeDiff.TotalSeconds.ShouldBe(seconds, timeTolerance);
    RootNode.RemoveChild(testNode);
    testNode.QueueFree();
  }
}
