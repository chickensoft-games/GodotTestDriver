namespace Chickensoft.GodotTestDriver.Tests;

using System;
using System.Threading.Tasks;
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
    public async Task TextChanges()
    {
        // WHEN
        // i type "hello" into the line edit
        await _lineEdit.Type("hello");
        // THEN
        // the text changes
        _lineEdit.Text.ShouldBe("hello");
    }

    [Test]
    public async Task SubmittingTextSendsSignal()
    {
        var awaiter = _lineEdit.GetSignalAwaiter(LineEdit.SignalName.TextSubmitted);
        // WHEN
        // i type "hello" into the line edit and press enter
        await _lineEdit.Submit("hello");
        // THEN
        // the text changes
        _lineEdit.Text.ShouldBe("hello");
        // and the text submitted signal is emitted
        awaiter.IsCompleted.ShouldBeTrue();
    }

    [Test]
    public async Task DisabledLineEditCannotBeEdited()
    {
        // SETUP
        _lineEdit.PresentRoot.Editable = false;

        // WHEN
        // i try to type into the disabled line edit
        // THEN
        // an exception is thrown
        await Should.ThrowAsync<InvalidOperationException>(async () => await _lineEdit.Type("hello"));
    }

    [Test]
    public async Task DisabledLineEditCannotBeSubmitted()
    {
        // SETUP
        _lineEdit.PresentRoot.Editable = false;

        // WHEN
        // i try to submit text into the disabled line edit
        // THEN
        // an exception is thrown
        await Should.ThrowAsync<InvalidOperationException>(async () => await _lineEdit.Submit("hello"));
    }

    [Test]
    public async Task InvisibleLineEditCannotBeEdited()
    {
        // SETUP
        _lineEdit.PresentRoot.Visible = false;

        // WHEN
        // i try to type into the invisible line edit
        // THEN
        // an exception is thrown
        await Should.ThrowAsync<InvalidOperationException>(async () => await _lineEdit.Type("hello"));
    }

    [Test]
    public async Task InvisibleLineEditCannotBeSubmitted()
    {
        // SETUP
        _lineEdit.PresentRoot.Visible = false;

        // WHEN
        // i try to submit text into the invisible line edit
        // THEN
        // an exception is thrown
        await Should.ThrowAsync<InvalidOperationException>(async () => await _lineEdit.Submit("hello"));
    }
}
