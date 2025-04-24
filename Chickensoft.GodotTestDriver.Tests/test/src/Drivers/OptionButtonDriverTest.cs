namespace Chickensoft.GodotTestDriver.Tests.Drivers;

using System;
using System.Linq;
using Chickensoft.GoDotTest;
using Godot;
using GodotTestDriver.Drivers;
using Shouldly;

public class OptionButtonDriverTest : DriverTest
{
    private readonly OptionButtonDriver _optionButton;

    public OptionButtonDriverTest(Node testScene) : base(testScene)
    {
        _optionButton = new OptionButtonDriver(() => RootNode.GetNode<OptionButton>("OptionButton"));
    }

    [Test]
    public void InspectionWorks()
    {
        // WHEN
        // everything is set up

        // THEN
        // we should have two selectable items
        _optionButton.SelectableItems.Count().ShouldBe(2);

        // named "Normal Item 1" and "Normal Item 2"
        _optionButton.SelectableItems.First().ShouldBe("Normal Item 1");
        _optionButton.SelectableItems.Last().ShouldBe("Normal Item 2");
    }

    [Test]
    public void SelectingAnItemWorks()
    {
        // WHEN
        // we select the first item
        var signalAwaiter = _optionButton.GetSignalAwaiter(OptionButton.SignalName.ItemSelected);
        _optionButton.SelectItemWithText("Normal Item 1");

        // THEN
        // the first item is selected
        _optionButton.SelectedItem.ShouldBe("Normal Item 1");

        // and the signal is emitted
        signalAwaiter.IsCompleted.ShouldBeTrue();
    }

    [Test]
    public void SelectingANonExistingItemThrowsException()
    {
        // WHEN
        // we select a non-existing item
        var signalAwaiter = _optionButton.GetSignalAwaiter(OptionButton.SignalName.ItemSelected);
        var exception = Should.Throw<Exception>(() => _optionButton.SelectItemWithText("Non-existing Item"));

        // THEN
        // the exception is thrown
        exception.Message.ShouldContain("does not contain");

        // and the signal is not emitted
        signalAwaiter.IsCompleted.ShouldBeFalse();
    }

    [Test]
    public void SelectingADisabledItemThrowsException()
    {
        // WHEN
        // we select a disabled item
        var signalAwaiter = _optionButton.GetSignalAwaiter(OptionButton.SignalName.ItemSelected);
        var exception = Should.Throw<Exception>(() => _optionButton.SelectItemWithText("Disabled Item"));

        // THEN
        // the exception is thrown
        exception.Message.ShouldContain("is not selectable");

        // and the signal is not emitted
        signalAwaiter.IsCompleted.ShouldBeFalse();
    }

    [Test]
    public void SelectingASeparatorThrowsException()
    {
        // WHEN
        // we select a separator
        var signalAwaiter = _optionButton.GetSignalAwaiter(OptionButton.SignalName.ItemSelected);
        var exception = Should.Throw<Exception>(() => _optionButton.SelectItemWithText("Separator"));

        // THEN
        // the exception is thrown
        exception.Message.ShouldContain("is not selectable");

        // and the signal is not emitted
        signalAwaiter.IsCompleted.ShouldBeFalse();
    }
}
