namespace Chickensoft.GodotTestDriver.Tests.Drivers;

using System;
using System.Linq;
using Chickensoft.GoDotTest;
using Godot;
using GodotTestDriver.Drivers;
using Shouldly;

public class ItemListDriverTest : DriverTest
{
  private readonly ItemListDriver _itemList;

  public ItemListDriverTest(Node testScene) : base(testScene)
  {
    _itemList = new ItemListDriver(() =>
      RootNode.GetNode<ItemList>("ItemList"));
  }

  [Test]
  public void InspectionWorks()
  {
    // WHEN
    // everything is set up

    // THEN
    // we should have two selectable items
    _itemList.SelectableItems.Count.ShouldBe(2);

    // named "Normal Item 1" and "Normal Item 2"
    _itemList.SelectableItems[0].ShouldBe("Normal Item 1");
    _itemList.SelectableItems.Last().ShouldBe("Normal Item 2");

    // and we should have a total of 4 items
    _itemList.ItemCount.ShouldBe(4);
  }

  [Test]
  public void SelectingItemsWorks()
  {
    // WHEN
    // we select the first item
    var signalAwaiter = _itemList.GetSignalAwaiter(
      ItemList.SignalName.ItemSelected
    );
    _itemList.SelectItemWithText("Normal Item 1");

    // THEN
    // the first item is selected
    _itemList.SelectedItems.Count.ShouldBe(1);
    _itemList.SelectedItems[0].ShouldBe("Normal Item 1");

    // and the signal is emitted
    signalAwaiter.IsCompleted.ShouldBeTrue();

    // WHEN
    // we select the second item
    var signalAwaiter2 = _itemList.GetSignalAwaiter(
      ItemList.SignalName.ItemSelected
    );
    _itemList.SelectItemWithText("Normal Item 2");

    // THEN
    // the second item is selected
    _itemList.SelectedItems.Count.ShouldBe(1);
    _itemList.SelectedItems[0].ShouldBe("Normal Item 2");

    // and the signal is emitted
    signalAwaiter2.IsCompleted.ShouldBeTrue();
  }

  [Test]
  public void MultiSelectionWorks()
  {
    // WHEN
    // we select the first item
    _itemList.SelectItemWithText("Normal Item 1");

    // and adding the second item
    _itemList.SelectItemWithText("Normal Item 2", true);

    // THEN
    // both items are selected
    _itemList.SelectedItems.Count.ShouldBe(2);
    _itemList.SelectedItems[0].ShouldBe("Normal Item 1");
    _itemList.SelectedItems.Last().ShouldBe("Normal Item 2");
  }

  [Test]
  public void MassSelectionWorks()
  {
    // WHEN
    // we select all items
    _itemList.SelectItemsWithText("Normal Item 1", "Normal Item 2");

    // THEN
    // both items are selected
    _itemList.SelectedItems.Count.ShouldBe(2);
    _itemList.SelectedItems[0].ShouldBe("Normal Item 1");
    _itemList.SelectedItems.Last().ShouldBe("Normal Item 2");
  }

  [Test]
  public void DeselectionWorks()
  {
    // WHEN
    // we select the first item
    _itemList.SelectItemWithText("Normal Item 1");

    // and deselect it
    _itemList.DeselectItemWithText("Normal Item 1");

    // THEN
    // no items are selected
    _itemList.SelectedItems.Count.ShouldBe(0);
  }

  [Test]
  public void MassDeselectionWorks()
  {
    // WHEN
    // we select the two items
    _itemList.SelectItemWithText("Normal Item 1");
    _itemList.SelectItemWithText("Normal Item 2", true);

    // and deselect all
    _itemList.DeselectAll();

    // THEN
    // no items are selected
    _itemList.SelectedItems.Count.ShouldBe(0);
  }

  [Test]
  public void ActivatingItemsWorks()
  {
    // WHEN
    // we activate the first item
    var signalAwaiter = _itemList.GetSignalAwaiter(
      ItemList.SignalName.ItemActivated
    );
    _itemList.ActivateItemWithText("Normal Item 1");

    // THEN
    // the signal is emitted
    signalAwaiter.IsCompleted.ShouldBeTrue();
  }

  [Test]
  public void SelectingDisabledItemsDoesNotWork() =>
    // WHEN
    // we try to select a disabled item, an InvalidOperationException should be thrown
    Should.Throw<InvalidOperationException>(
      () => _itemList.SelectItemWithText("Disabled Item")
    );

  [Test]
  public void SelectingNonExistingItemsDoesNotWork() =>
    // WHEN
    // we try to select a non-existing item, an InvalidOperationException should be thrown
    Should.Throw<InvalidOperationException>(
      () => _itemList.SelectItemWithText("Non-existing Item")
    );

  [Test]
  public void SelectingUnselectableItemsDoesNotWork() =>
    // WHEN
    // we try to select an unselectable item, an InvalidOperationException should be thrown
    Should.Throw<InvalidOperationException>(
      () => _itemList.SelectItemWithText("NonSelectable Item")
    );
}
