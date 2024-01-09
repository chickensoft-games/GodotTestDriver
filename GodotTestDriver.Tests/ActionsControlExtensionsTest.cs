namespace GodotTestDriver.Tests;

using System;
using System.Threading.Tasks;
using Chickensoft.GoDotTest;
using Godot;
using GodotTestDriver.Input;
using GodotTestDriver.Util;
using JetBrains.Annotations;
using Shouldly;

[UsedImplicitly]
public class ActionsControlExtensionsTest : DriverTest
{
    private const string TestAction = "test_action";

    public ActionsControlExtensionsTest(Node testScene) : base(testScene)
    {
    }

    [Test]
    public async Task StartActionSetsGlobalActionPressed()
    {
        Input.IsActionPressed(TestAction).ShouldBeFalse();
        await RootNode.StartAction(TestAction);
        Input.IsActionPressed(TestAction).ShouldBeTrue();
        await RootNode.EndAction(TestAction);
    }

    [Test]
    public async Task EndActionUnsetsGlobalActionPressed()
    {
        await RootNode.StartAction(TestAction);
        await RootNode.EndAction(TestAction);
        Input.IsActionPressed(TestAction).ShouldBeFalse();
    }
}
