namespace Chickensoft.GodotTestDriver.Tests.Drivers;

using System;
using Chickensoft.GoDotTest;
using Godot;
using GodotTestDriver.Drivers;
using Shouldly;

public class TextEditDriverTest : DriverTest
{
    private readonly TextEditDriver _textEdit;

    public TextEditDriverTest(Node testScene) : base(testScene)
    {
        _textEdit = new TextEditDriver(() => RootNode.GetNode<TextEdit>("TextEdit"));
    }

    [Test]
    public void TypingWorks()
    {
        // get the signal awaiter
        var signalAwaiter = _textEdit.GetSignalAwaiter(TextEdit.SignalName.TextChanged);

        // WHEN
        // i type "Hello World!" into the text edit
        _textEdit.Type("Hello World!");

        // THEN
        // the text edit text is "Hello World!"
        _textEdit.Text.ShouldBe("Hello World!");

        // and the text changed signal was emitted
        signalAwaiter.IsCompleted.ShouldBeTrue();
    }

    [Test]
    public void TypingThrowsExceptionIfNotEditable()
    {
        // GIVEN
        // the text edit is not editable
        _textEdit.PresentRoot.Editable = false;

        // WHEN
        // i try to type "Hello World!" into the text edit
        var exception = Should.Throw<InvalidOperationException>(() => _textEdit.Type("Hello World!"));

        // THEN
        // the exception message is correct
        exception.Message.ShouldContain("it is read-only");
    }

    [Test]
    public void ReadOnlyStatusIsProperlyDetected()
    {
        // GIVEN
        // the text edit is editable
        _textEdit.PresentRoot.Editable = true;

        // THEN
        // the text edit is not read-only
        _textEdit.ReadOnly.ShouldBeFalse();

        // GIVEN
        // the text edit is not editable
        _textEdit.PresentRoot.Editable = false;

        // THEN
        // the text edit is read-only
        _textEdit.ReadOnly.ShouldBeTrue();
    }
}
