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
        RootNode.SetControllerSingleAxis(TestAxis1, 0.8f);
        Input.GetActionStrength(TestAxis1).ShouldBe(0.8f);
        RootNode.SetControllerSingleAxis(TestAxis1, 0.25f);
        Input.GetActionStrength(TestAxis1).ShouldBe(0.25f);
        RootNode.SetControllerSingleAxis(TestAxis1, 0.8f);
        Input.GetActionStrength(TestAxis1).ShouldBe(0.8f);
        RootNode.ReleaseControllerSingleAxis(TestAxis1);
    }

    [Test]
    public void ReleaseSingleAxisSetsStrengthToZero()
    {
        RootNode.SetControllerSingleAxis(TestAxis1, 1);
        RootNode.ReleaseControllerSingleAxis(TestAxis1);
        Input.GetActionStrength(TestAxis1).ShouldBe(0);
    }

    [Test]
    public void SingleAxisValueClampsToZero()
    {
        RootNode.SetControllerSingleAxis(TestAxis1, -1);
        Input.GetActionStrength(TestAxis1).ShouldBe(0);
        RootNode.ReleaseControllerSingleAxis(TestAxis1);
    }

    [Test]
    public void SingleAxisValueClampsToOne()
    {
        RootNode.SetControllerSingleAxis(TestAxis1, 2);
        Input.GetActionStrength(TestAxis1).ShouldBe(1);
        RootNode.ReleaseControllerSingleAxis(TestAxis1);
    }

    [Test]
    public void SetSingleAxisToZeroReleasesAction()
    {
        RootNode.SetControllerSingleAxis(TestAxis1, 0);
        Input.IsActionPressed(TestAxis1).ShouldBeFalse();
        RootNode.ReleaseControllerSingleAxis(TestAxis1);
    }

    [Test]
    public void SetDoubleAxisSetsBothStrengthValues()
    {
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0f);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0f);
        RootNode.SetControllerDoubleAxis(TestAxis2Negative, TestAxis2Positive, 0.8f);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0f);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0.8f);
        RootNode.SetControllerDoubleAxis(TestAxis2Negative, TestAxis2Positive, 0.25f);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0f);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0.25f);
        RootNode.SetControllerDoubleAxis(TestAxis2Negative, TestAxis2Positive, 0.8f);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0f);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0.8f);
        RootNode.SetControllerDoubleAxis(TestAxis2Negative, TestAxis2Positive, -0.8f);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0.8f);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0f);
        RootNode.SetControllerDoubleAxis(TestAxis2Negative, TestAxis2Positive, -0.25f);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0.25f);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0f);
        RootNode.SetControllerDoubleAxis(TestAxis2Negative, TestAxis2Positive, -0.8f);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0.8f);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0f);
        RootNode.ReleaseControllerDoubleAxis(TestAxis2Negative, TestAxis2Positive);
    }

    [Test]
    public void ReleaseDoubleAxisSetsStrengthsToZero()
    {
        RootNode.SetControllerDoubleAxis(TestAxis2Negative, TestAxis2Positive, 1);
        RootNode.ReleaseControllerDoubleAxis(TestAxis2Negative, TestAxis2Positive);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0);
        RootNode.SetControllerDoubleAxis(TestAxis2Negative, TestAxis2Positive, -1);
        RootNode.ReleaseControllerDoubleAxis(TestAxis2Negative, TestAxis2Positive);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0);
    }

    [Test]
    public void DoubleAxisValueClampsToNegativeOne()
    {
        RootNode.SetControllerDoubleAxis(TestAxis2Negative, TestAxis2Positive, -2);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(1);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0);
        RootNode.ReleaseControllerDoubleAxis(TestAxis2Negative, TestAxis2Positive);
    }

    [Test]
    public void DoubleAxisValueClampsToPositiveOne()
    {
        RootNode.SetControllerDoubleAxis(TestAxis2Negative, TestAxis2Positive, 2);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(1);
        RootNode.ReleaseControllerDoubleAxis(TestAxis2Negative, TestAxis2Positive);
    }

    [Test]
    public void SetDoubleAxisToZeroReleasesActions()
    {
        RootNode.SetControllerDoubleAxis(TestAxis2Negative, TestAxis2Positive, 0);
        Input.IsActionPressed(TestAxis2Negative).ShouldBeFalse();
        Input.IsActionPressed(TestAxis2Positive).ShouldBeFalse();
        RootNode.ReleaseControllerDoubleAxis(TestAxis2Negative, TestAxis2Positive);
    }
}
