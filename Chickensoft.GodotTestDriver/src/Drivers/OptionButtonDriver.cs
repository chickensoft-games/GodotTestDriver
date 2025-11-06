namespace Chickensoft.GodotTestDriver.Drivers;

using System;
using System.Collections.Generic;
using Godot;

/// <summary>
/// Driver for <see cref="OptionButton"/> controls.
/// </summary>
/// <typeparam name="T">OptionButton type.</typeparam>
public class OptionButtonDriver<T> : BaseButtonDriver<T> where T : OptionButton
{
  /// <summary>
  /// Creates a new generic OptionButtonDriver.
  /// </summary>
  /// <param name="producer">
  /// Producer that creates a OptionButton subclass.
  /// </param>
  /// <param name="description">Driver description.</param>
  public OptionButtonDriver(Func<T> producer, string description = "")
    : base(producer, description)
  {
  }

  /// <summary>
  /// Returns a list of all items in the option button.
  /// </summary>
  public IEnumerable<string> Items
  {
    get
    {
      var uiControl = PresentRoot;
      for (var i = 0; i < uiControl.ItemCount; i++)
      {
        yield return uiControl.GetItemText(i);
      }
    }
  }

  /// <summary>
  /// Returns a list of all items in the option button that are currently
  /// selectable (e.g. not disabled).
  /// </summary>
  public IEnumerable<string> SelectableItems
  {
    get
    {
      var uiControl = PresentRoot;

      for (var i = 0; i < uiControl.ItemCount; i++)
      {
        if (!uiControl.IsItemDisabled(i) && !uiControl.IsItemSeparator(i))
        {
          yield return uiControl.GetItemText(i);
        }
      }
    }
  }

  /// <summary>
  /// Returns the currently selected item, or null if no item is selected.
  /// </summary>
  public string? SelectedItem
  {
    get
    {
      var uiControl = PresentRoot;
      return uiControl.Selected < 0
        ? null
        : uiControl.GetItemText(uiControl.Selected);
    }
  }

  /// <summary>
  /// Returns the currently selected item index, or -1 if no item is selected.
  /// </summary>
  public int SelectedIndex => PresentRoot.Selected;

  /// <summary>
  /// Returns the currently selected item ID, or -1 if no item is selected.
  /// </summary>
  public int SelectedId =>
    PresentRoot.Selected < 0
      ? -1
      : PresentRoot.GetItemId(PresentRoot.Selected);

  /// <summary>
  /// Returns the index of the item with the given text. Throws an exception if
  /// the item is not found or not selectable.
  /// </summary>
  /// <param name="text">Option label.</param>
  /// <exception cref="InvalidOperationException" />
  private int IndexOf(string text)
  {
    var uiControl = VisibleRoot;
    for (var i = 0; i < uiControl.ItemCount; i++)
    {
      if (uiControl.GetItemText(i) == text)
      {
        if (uiControl.IsItemDisabled(i) || uiControl.IsItemSeparator(i))
        {
          throw new InvalidOperationException(
            ErrorMessage($"Item with text '{text}' is not selectable.")
          );
        }

        return i;
      }
    }

    throw new InvalidOperationException(
      ErrorMessage($"Option button does not contain item with name '{text}'.")
    );
  }

  /// <summary>
  /// Selects an item with the given text.
  /// </summary>
  /// <param name="text">Option label.</param>
  public void SelectItemWithText(string text)
  {
    var uiControl = VisibleRoot;
    var index = IndexOf(text);
    uiControl.Select(index);
    // calling this function will not emit the signal so we need to do this
    // ourselves
    uiControl.EmitSignal(OptionButton.SignalName.ItemSelected, index);
  }

  /// <summary>
  /// Selects an item with the given ID.
  /// </summary>
  /// <param name="id">Option id.</param>
  /// <exception cref="InvalidOperationException" />
  public void SelectItemWithId(int id)
  {
    var uiControl = VisibleRoot;

    for (var i = 0; i < uiControl.ItemCount; i++)
    {
      if (uiControl.GetItemId(i) != id)
      {
        continue;
      }

      if (uiControl.IsItemDisabled(i))
      {
        throw new InvalidOperationException(
          ErrorMessage($"Item with ID '{id}' is not selectable.")
        );
      }

      uiControl.Select(i);
      // calling this function will not emit the signal so we need to do this
      // ourselves
      uiControl.EmitSignal(OptionButton.SignalName.ItemSelected, i);
      return;
    }

    throw new InvalidOperationException(
      ErrorMessage($"Option button does not contain item with ID '{id}'.")
    );
  }
}

/// <summary>
/// Driver for <see cref="OptionButton"/> controls.
/// </summary>
public sealed class OptionButtonDriver : OptionButtonDriver<OptionButton>
{
  /// <summary>
  /// Creates a new OptionButtonDriver.
  /// </summary>
  /// <param name="producer">
  /// Producer that creates a OptionButton subclass.
  /// </param>
  /// <param name="description">Driver description.</param>
  public OptionButtonDriver(
    Func<OptionButton> producer,
    string description = ""
  )
    : base(producer, description)
  {
  }
}
