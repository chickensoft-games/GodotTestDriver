namespace Chickensoft.GodotTestDriver.Drivers;

using System;
using System.Collections.Generic;
using Godot;

/// <summary>
/// Driver for a popup menu.
/// </summary>
/// <typeparam name="T">PopupMenu type.</typeparam>
public class PopupMenuDriver<T> : WindowDriver<T> where T : PopupMenu
{
  /// <summary>
  /// Creates a new generic PopupMenuDriver.
  /// </summary>
  /// <param name="producer">Producer that creates a PopupMenu subclass.</param>
  /// <param name="description">Driver description.</param>
  public PopupMenuDriver(Func<T> producer, string description = "")
    : base(producer, description)
  {
  }

  /// <summary>
  /// Returns the amount of items in the popup menu.
  /// </summary>
  public int ItemCount => PresentRoot.ItemCount;

  /// <summary>
  /// Returns the text of the items in the popup menu.
  /// </summary>
  public IEnumerable<string> Items
  {
    get
    {
      for (var i = 0; i < ItemCount; i++)
      {
        yield return PresentRoot.GetItemText(i);
      }
    }
  }

  /// <summary>
  /// Returns the text of the items in the popup menu which are currently
  /// selectable (e.g. not disabled and no separator).
  /// </summary>
  public IEnumerable<string> SelectableItems
  {
    get
    {
      for (var i = 0; i < ItemCount; i++)
      {
        if (PresentRoot.IsItemDisabled(i) || PresentRoot.IsItemSeparator(i))
        {
          continue;
        }
        yield return PresentRoot.GetItemText(i);
      }
    }
  }

  /// <summary>
  /// Returns whether the item at the given index is checked.
  /// </summary>
  /// <param name="index">Menu item index.</param>
  public bool IsItemChecked(int index)
  {
    VerifyIndex(index);
    return PresentRoot.IsItemChecked(index);
  }

  /// <summary>
  /// Returns whether the item with the given text is checked.
  /// </summary>
  /// <param name="text">Menu item label.</param>
  /// <exception cref="InvalidOperationException"/>
  public bool IsItemChecked(string text)
  {
    var element = PresentRoot;

    for (var i = 0; i < element.ItemCount; i++)
    {
      if (element.GetItemText(i) == text)
      {
        return element.IsItemChecked(i);
      }
    }

    throw new InvalidOperationException($"Item with text '{text}' not found.");
  }

  /// <summary>
  /// Returns whether the item at the given index is disabled.
  /// </summary>
  /// <param name="index">Menu item index.</param>
  public bool IsItemDisabled(int index)
  {
    VerifyIndex(index);
    return PresentRoot.IsItemDisabled(index);
  }

  /// <summary>
  /// Checks if the item with the given text is disabled.
  /// </summary>
  /// <param name="text">Menu item label.</param>
  /// <exception cref="InvalidOperationException"/>
  public bool IsItemDisabled(string text)
  {
    var element = PresentRoot;

    for (var i = 0; i < element.ItemCount; i++)
    {
      if (element.GetItemText(i) == text)
      {
        return element.IsItemDisabled(i);
      }
    }

    throw new InvalidOperationException($"Item with text '{text}' not found.");
  }

  /// <summary>
  /// Returns whether the item at the given index is a separator.
  /// </summary>
  /// <param name="index">Menu item index.</param>
  public bool IsItemSeparator(int index)
  {
    VerifyIndex(index);
    return PresentRoot.IsItemSeparator(index);
  }

  /// <summary>
  /// Returns whether the item with the given text is a separator.
  /// </summary>
  /// <param name="text">Menu item label.</param>
  /// <exception cref="InvalidOperationException"/>
  public bool IsItemSeparator(string text)
  {
    var element = PresentRoot;

    for (var i = 0; i < element.ItemCount; i++)
    {
      if (element.GetItemText(i) == text)
      {
        return element.IsItemSeparator(i);
      }
    }

    throw new InvalidOperationException($"Item with text {text} not found.");
  }

  /// <summary>
  /// Returns the ID of the item at the given index.
  /// </summary>
  /// <param name="index">Menu item index.</param>
  public int GetItemId(int index)
  {
    VerifyIndex(index);
    return PresentRoot.GetItemId(index);
  }

  /// <summary>
  /// Selects the item at the given index.
  /// </summary>
  /// <param name="index">Menu item index.</param>
  /// <exception cref="InvalidOperationException"/>
  public void SelectItemAtIndex(int index)
  {
    var popup = VisibleRoot;
    VerifyIndex(index);

    // verify that item is not disabled
    if (popup.IsItemDisabled(index))
    {
      throw new InvalidOperationException(
          $"Item at index {index} is disabled and cannot be selected.");
    }

    // verify that item is not a separator
    if (popup.IsItemSeparator(index))
    {
      throw new InvalidOperationException(
          $"Item at index {index} is a separator and cannot be selected.");
    }

    // select item
    // ideally we would use a mouse click here but since the API does not
    // provide the position of each entry, we have to fake it.
    popup.EmitSignal(PopupMenu.SignalName.IndexPressed, index);
    popup.Hide();
  }

  /// <summary>
  /// Verifies that the given index is valid.
  /// </summary>
  /// <param name="index">Menu item index.</param>
  /// <exception cref="IndexOutOfRangeException"/>
  private void VerifyIndex(int index)
  {
    // verify index is in range
    if (index < 0 || index >= ItemCount)
    {
      throw new ArgumentOutOfRangeException(
        nameof(index),
        $"Index {index} is out of range for popup menu with {ItemCount} items."
      );
    }
  }

  /// <summary>
  /// Selects the item with the given ID.
  /// </summary>
  /// <param name="id">Menu item id.</param>
  /// <exception cref="InvalidOperationException" />
  public void SelectItemWithId(int id)
  {
    var popup = PresentRoot;
    for (var i = 0; i < ItemCount; i++)
    {
      if (popup.GetItemId(i) != id)
      {
        continue;
      }
      SelectItemAtIndex(i);
      return;
    }

    throw new InvalidOperationException(
        $"No item with id {id} found in popup menu.");
  }

  /// <summary>
  /// Selects the item with the given text. If multiple items have the same
  /// text, the first one is selected.
  /// </summary>
  /// <param name="text">Menu item label.</param>
  /// <exception cref="InvalidOperationException"/>
  public void SelectItemWithText(string text)
  {
    var popup = PresentRoot;
    for (var i = 0; i < ItemCount; i++)
    {
      if (popup.GetItemText(i) != text)
      {
        continue;
      }
      SelectItemAtIndex(i);
      return;
    }

    throw new InvalidOperationException(
        $"No item with text {text} found in popup menu.");
  }
}

/// <summary>
/// Driver for a popup menu.
/// </summary>
public sealed class PopupMenuDriver : PopupMenuDriver<PopupMenu>
{
  /// <summary>
  /// Creates a new generic PopupMenuDriver.
  /// </summary>
  /// <param name="producer">Producer that creates a PopupMenu subclass.</param>
  /// <param name="description">Driver description.</param>
  public PopupMenuDriver(Func<PopupMenu> producer, string description = "")
    : base(producer, description)
  {
  }
}
