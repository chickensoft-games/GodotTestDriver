namespace Chickensoft.GodotTestDriver.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.GodotTestDriver.Input;
using Godot;
using JetBrains.Annotations;
using Shouldly;

[UsedImplicitly]
public partial class ControllerInputExtensionsTest : DriverTest
{
    private const string TestAxis1 = "test_controller_axis1";
    private const string TestAxis2Positive = "test_controller_axis2_positive";
    private const string TestAxis2Negative = "test_controller_axis2_negative";

    public ControllerInputExtensionsTest(Node testScene) : base(testScene)
    {
    }

    [Test]
    public void SetSingleAxisSetsStrength()
    {
        Input.GetActionStrength(TestAxis1).ShouldBe(0f);
        RootNode.SetControllerSingleAxisInput(TestAxis1, 0.8f);
        Input.GetActionStrength(TestAxis1).ShouldBe(0.8f);
        RootNode.SetControllerSingleAxisInput(TestAxis1, 0.25f);
        Input.GetActionStrength(TestAxis1).ShouldBe(0.25f);
        RootNode.SetControllerSingleAxisInput(TestAxis1, 0.8f);
        Input.GetActionStrength(TestAxis1).ShouldBe(0.8f);
        RootNode.EndControllerSingleAxisInput(TestAxis1);
    }

    [Test]
    public void EndSingleAxisSetsStrengthToZero()
    {
        RootNode.SetControllerSingleAxisInput(TestAxis1, 1);
        RootNode.EndControllerSingleAxisInput(TestAxis1);
        Input.GetActionStrength(TestAxis1).ShouldBe(0);
    }

    [Test]
    public void SingleAxisValueClampsToZero()
    {
        RootNode.SetControllerSingleAxisInput(TestAxis1, -1);
        Input.GetActionStrength(TestAxis1).ShouldBe(0);
        RootNode.EndControllerSingleAxisInput(TestAxis1);
    }

    [Test]
    public void SingleAxisValueClampsToOne()
    {
        RootNode.SetControllerSingleAxisInput(TestAxis1, 2);
        Input.GetActionStrength(TestAxis1).ShouldBe(1);
        RootNode.EndControllerSingleAxisInput(TestAxis1);
    }

    [Test]
    public void SetSingleAxisToZeroEndsAction()
    {
        RootNode.SetControllerSingleAxisInput(TestAxis1, 0);
        Input.IsActionPressed(TestAxis1).ShouldBeFalse();
        RootNode.EndControllerSingleAxisInput(TestAxis1);
    }

    [Test]
    public void SetDoubleAxisSetsBothStrengthValues()
    {
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0f);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0f);
        RootNode.SetControllerDoubleAxisInput(TestAxis2Negative, TestAxis2Positive, 0.8f);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0f);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0.8f);
        RootNode.SetControllerDoubleAxisInput(TestAxis2Negative, TestAxis2Positive, 0.25f);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0f);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0.25f);
        RootNode.SetControllerDoubleAxisInput(TestAxis2Negative, TestAxis2Positive, 0.8f);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0f);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0.8f);
        RootNode.SetControllerDoubleAxisInput(TestAxis2Negative, TestAxis2Positive, -0.8f);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0.8f);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0f);
        RootNode.SetControllerDoubleAxisInput(TestAxis2Negative, TestAxis2Positive, -0.25f);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0.25f);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0f);
        RootNode.SetControllerDoubleAxisInput(TestAxis2Negative, TestAxis2Positive, -0.8f);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0.8f);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0f);
        RootNode.EndControllerDoubleAxisInput(TestAxis2Negative, TestAxis2Positive);
    }

    [Test]
    public void EndDoubleAxisSetsStrengthsToZero()
    {
        RootNode.SetControllerDoubleAxisInput(TestAxis2Negative, TestAxis2Positive, 1);
        RootNode.EndControllerDoubleAxisInput(TestAxis2Negative, TestAxis2Positive);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0);
        RootNode.SetControllerDoubleAxisInput(TestAxis2Negative, TestAxis2Positive, -1);
        RootNode.EndControllerDoubleAxisInput(TestAxis2Negative, TestAxis2Positive);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0);
    }

    [Test]
    public void DoubleAxisValueClampsToNegativeOne()
    {
        RootNode.SetControllerDoubleAxisInput(TestAxis2Negative, TestAxis2Positive, -2);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(1);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0);
        RootNode.EndControllerDoubleAxisInput(TestAxis2Negative, TestAxis2Positive);
    }

    [Test]
    public void DoubleAxisValueClampsToPositiveOne()
    {
        RootNode.SetControllerDoubleAxisInput(TestAxis2Negative, TestAxis2Positive, 2);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(1);
        RootNode.EndControllerDoubleAxisInput(TestAxis2Negative, TestAxis2Positive);
    }

    [Test]
    public void SetDoubleAxisToZeroEndsActions()
    {
        RootNode.SetControllerDoubleAxisInput(TestAxis2Negative, TestAxis2Positive, 0);
        Input.IsActionPressed(TestAxis2Negative).ShouldBeFalse();
        Input.IsActionPressed(TestAxis2Positive).ShouldBeFalse();
        RootNode.EndControllerDoubleAxisInput(TestAxis2Negative, TestAxis2Positive);
    }
}
