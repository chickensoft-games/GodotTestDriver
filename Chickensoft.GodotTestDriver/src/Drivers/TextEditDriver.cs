namespace Chickensoft.GodotTestDriver.Drivers;

using System;
using Godot;

/// <summary>
/// Driver for the <see cref="TextEdit"/> control.
/// </summary>
/// <typeparam name="T">TextEdit type.</typeparam>
/// [PublicAPI]
public class TextEditDriver<T> : ControlDriver<T> where T : TextEdit
{
  /// <summary>
  /// Creates a new generic TextEditDriver.
  /// </summary>
  /// <param name="producer">Producer that creates a TextEdit subclass.</param>
  /// <param name="description">Driver description.</param>
  public TextEditDriver(Func<T> producer, string description = "")
    : base(producer, description)
  {
  }

  /// <summary>
  /// Text contents.
  /// </summary>
  public string Text => PresentRoot.Text;

  /// <summary>
  /// Whether the text edit is currently editable.
  /// </summary>
  public bool ReadOnly => !PresentRoot.Editable;

  /// <summary>
  /// Types the given text into the text edit. Existing text will be overwritten.
  /// </summary>
  /// <param name="text">Text to input.</param>
  /// <exception cref="InvalidOperationException" />
  public void Type(string text)
  {
    if (ReadOnly)
    {
      throw new InvalidOperationException(
        ErrorMessage("Cannot type text into TextEdit because it is read-only.")
      );
    }

    var edit = VisibleRoot;
    ClickCenter();
    edit.Text = text;
    edit.EmitSignal(TextEdit.SignalName.TextChanged, text);
  }
}

/// <summary>
/// Driver for the <see cref="TextEdit"/> control.
/// </summary>
public class TextEditDriver : TextEditDriver<TextEdit>
{
  /// <summary>
  /// Creates a new TextEditDriver.
  /// </summary>
  /// <param name="producer">Producer that creates a TextEdit subclass.</param>
  /// <param name="description">Driver description.</param>
  public TextEditDriver(Func<TextEdit> producer, string description = "")
    : base(producer, description)
  {
  }
}
