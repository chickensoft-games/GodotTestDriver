namespace Chickensoft.GodotTestDriver.Tests;

using System;
using Chickensoft.GoDotTest;
using Godot;
using GodotTestDriver.Drivers;
using Shouldly;

public class LineEditDriverTest : DriverTest
{
    private readonly LineEditDriver _lineEdit;

    public LineEditDriverTest(Node testScene) : base(testScene)
    {
        _lineEdit = new LineEditDriver(() => RootNode.GetNode<LineEdit>("LineEdit"));
    }

    [Test]
    public void TextChanges()
    {
        // WHEN
        // i type "hello" into the line edit
        _lineEdit.Type("hello");
        // THEN
        // the text changes
        _lineEdit.Text.ShouldBe("hello");
    }

    [Test]
    public void SubmittingTextSendsSignal()
    {
        var awaiter = _lineEdit.GetSignalAwaiter(LineEdit.SignalName.TextSubmitted);
        // WHEN
        // i type "hello" into the line edit and press enter
        _lineEdit.Submit("hello");
        // THEN
        // the text changes
        _lineEdit.Text.ShouldBe("hello");
        // and the text submitted signal is emitted
        awaiter.IsCompleted.ShouldBeTrue();
    }

    [Test]
    public void DisabledLineEditCannotBeEdited()
    {
        // SETUP
        _lineEdit.PresentRoot.Editable = false;

        // WHEN
        // i try to type into the disabled line edit
        // THEN
        // an exception is thrown
        Should.Throw<InvalidOperationException>(() => _lineEdit.Type("hello"));
    }

    [Test]
    public void DisabledLineEditCannotBeSubmitted()
    {
        // SETUP
        _lineEdit.PresentRoot.Editable = false;

        // WHEN
        // i try to submit text into the disabled line edit
        // THEN
        // an exception is thrown
        Should.Throw<InvalidOperationException>(() => _lineEdit.Submit("hello"));
    }

    [Test]
    public void InvisibleLineEditCannotBeEdited()
    {
        // SETUP
        _lineEdit.PresentRoot.Visible = false;

        // WHEN
        // i try to type into the invisible line edit
        // THEN
        // an exception is thrown
        Should.Throw<InvalidOperationException>(() => _lineEdit.Type("hello"));
    }

    [Test]
    public void InvisibleLineEditCannotBeSubmitted()
    {
        // SETUP
        _lineEdit.PresentRoot.Visible = false;

        // WHEN
        // i try to submit text into the invisible line edit
        // THEN
        // an exception is thrown
        Should.Throw<InvalidOperationException>(() => _lineEdit.Submit("hello"));
    }
}
