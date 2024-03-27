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
        RootNode.SetSingleAxisInput(TestAxis1, 0.8f);
        Input.GetActionStrength(TestAxis1).ShouldBe(0.8f);
        RootNode.SetSingleAxisInput(TestAxis1, 0.25f);
        Input.GetActionStrength(TestAxis1).ShouldBe(0.25f);
        RootNode.SetSingleAxisInput(TestAxis1, 0.8f);
        Input.GetActionStrength(TestAxis1).ShouldBe(0.8f);
        RootNode.EndSingleAxisInput(TestAxis1);
    }

    [Test]
    public void EndSingleAxisSetsStrengthToZero()
    {
        RootNode.SetSingleAxisInput(TestAxis1, 1);
        RootNode.EndSingleAxisInput(TestAxis1);
        Input.GetActionStrength(TestAxis1).ShouldBe(0);
    }

    [Test]
    public void SingleAxisValueClampsToZero()
    {
        RootNode.SetSingleAxisInput(TestAxis1, -1);
        Input.GetActionStrength(TestAxis1).ShouldBe(0);
        RootNode.EndSingleAxisInput(TestAxis1);
    }

    [Test]
    public void SingleAxisValueClampsToOne()
    {
        RootNode.SetSingleAxisInput(TestAxis1, 2);
        Input.GetActionStrength(TestAxis1).ShouldBe(1);
        RootNode.EndSingleAxisInput(TestAxis1);
    }

    [Test]
    public void SetBidirectionalAxisSetsBothStrengthValues()
    {
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0f);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0f);
        RootNode.SetBidirectionalAxisInput(TestAxis2Negative, TestAxis2Positive, 0.8f);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0f);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0.8f);
        RootNode.SetBidirectionalAxisInput(TestAxis2Negative, TestAxis2Positive, 0.25f);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0f);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0.25f);
        RootNode.SetBidirectionalAxisInput(TestAxis2Negative, TestAxis2Positive, 0.8f);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0f);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0.8f);
        RootNode.SetBidirectionalAxisInput(TestAxis2Negative, TestAxis2Positive, -0.8f);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0.8f);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0f);
        RootNode.SetBidirectionalAxisInput(TestAxis2Negative, TestAxis2Positive, -0.25f);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0.25f);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0f);
        RootNode.SetBidirectionalAxisInput(TestAxis2Negative, TestAxis2Positive, -0.8f);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0.8f);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0f);
        RootNode.EndBidirectionalAxisInput(TestAxis2Negative, TestAxis2Positive);
    }

    [Test]
    public void EndBidrectionalAxisSetsStrengthsToZero()
    {
        RootNode.SetBidirectionalAxisInput(TestAxis2Negative, TestAxis2Positive, 1);
        RootNode.EndBidirectionalAxisInput(TestAxis2Negative, TestAxis2Positive);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0);
        RootNode.SetBidirectionalAxisInput(TestAxis2Negative, TestAxis2Positive, -1);
        RootNode.EndBidirectionalAxisInput(TestAxis2Negative, TestAxis2Positive);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0);
    }

    [Test]
    public void BidirectionalAxisValueClampsToNegativeOne()
    {
        RootNode.SetBidirectionalAxisInput(TestAxis2Negative, TestAxis2Positive, -2);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(1);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(0);
        RootNode.EndBidirectionalAxisInput(TestAxis2Negative, TestAxis2Positive);
    }

    [Test]
    public void BidirectionalAxisValueClampsToPositiveOne()
    {
        RootNode.SetBidirectionalAxisInput(TestAxis2Negative, TestAxis2Positive, 2);
        Input.GetActionStrength(TestAxis2Negative).ShouldBe(0);
        Input.GetActionStrength(TestAxis2Positive).ShouldBe(1);
        RootNode.EndBidirectionalAxisInput(TestAxis2Negative, TestAxis2Positive);
    }
}
