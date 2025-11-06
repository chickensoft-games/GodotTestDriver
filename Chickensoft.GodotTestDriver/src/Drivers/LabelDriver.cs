namespace Chickensoft.GodotTestDriver.Drivers;

using System;
using Godot;

/// <summary>
/// Driver for the <see cref="Label"/> control.
/// </summary>
/// <typeparam name="T">Label type.</typeparam>
public class LabelDriver<T> : ControlDriver<T> where T : Label
{
  /// <summary>
  /// Creates a new generic LabelDriver.
  /// </summary>
  /// <param name="producer">Producer that creates a Label subclass.</param>
  /// <param name="description">Driver description.</param>
  public LabelDriver(Func<T> producer, string description = "")
    : base(producer, description)
  {
  }

  /// <summary>
  /// The current text of the label.
  /// </summary>
  public string Text => PresentRoot.Text;
}

/// <summary>
/// Driver for the <see cref="Label"/> control.
/// </summary>
public sealed class LabelDriver : LabelDriver<Label>
{
  /// <summary>
  /// Creates a new LabelDriver.
  /// </summary>
  /// <param name="producer">Producer that creates a Label subclass.</param>
  /// <param name="description">Driver description.</param>
  public LabelDriver(Func<Label> producer, string description = "")
    : base(producer, description)
  {
  }
}
