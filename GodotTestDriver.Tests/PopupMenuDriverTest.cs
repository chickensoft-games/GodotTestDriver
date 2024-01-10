namespace Chickensoft.GodotTestDriver.Tests;

using System.Linq;
using System.Threading.Tasks;
using Chickensoft.GoDotTest;
using Godot;
using GodotTestDriver.Drivers;
using Shouldly;

public class PopupMenuDriverTest : DriverTest
{
    private readonly PopupMenuDriver _popupMenu;
    private readonly ButtonDriver _button;

    public PopupMenuDriverTest(Node testScene) : base(testScene)
    {
        _popupMenu = new PopupMenuDriver(() => RootNode.GetNode<PopupMenu>("PopupMenu"));
        _button = new ButtonDriver(() => RootNode.GetNode<Button>("Button"));
    }

    [Test]
    public void HiddenCheckWorks()
    {
        // WHEN
        // everything is set up

        _popupMenu.IsVisible.ShouldBeFalse();
    }

    [Test]
    public void InspectionWorks()
    {
        // WHEN
        // we press the button
        _button.Press();

        // THEN
        // the popup menu is visible
        _popupMenu.IsVisible.ShouldBeTrue();

        // and we have two selectable items
        _popupMenu.SelectableItems.Count().ShouldBe(2);

        // named "Normal Item 1" and "Normal Item 2"
        _popupMenu.SelectableItems.First().ShouldBe("Normal Item 1");
        _popupMenu.SelectableItems.Last().ShouldBe("Normal Item 2");

        // and the second item should be checked
        _popupMenu.IsItemChecked("Normal Item 2").ShouldBeTrue();

        // and the "Separator" item should be a separator
        _popupMenu.IsItemSeparator("Separator").ShouldBeTrue();

        // and the "Disabled Item" item should be disabled
        _popupMenu.IsItemDisabled("Disabled Item").ShouldBeTrue();
    }

    // popup introspection shows two selectable items
    // named "Normal Item 1" and "Normal Item 2"

    [Test]
    public async Task SelectingAnItemWorks()
    {
        // WHEN
        // we press the button
        _button.Press();

        // and we select the first item
        var signalAwaiter = _popupMenu.GetSignalAwaiter(PopupMenu.SignalName.IndexPressed);
        await _popupMenu.SelectItemWithText("Normal Item 1");

        // THEN
        // the popup menu should be hidden
        _popupMenu.IsVisible.ShouldBeFalse();

        // and the signal is emitted
        signalAwaiter.IsCompleted.ShouldBeTrue();
        signalAwaiter.GetResult()[0].ShouldBe(0); // the first item is selected
    }
}
