namespace Chickensoft.GodotTestDriver.Drivers;

using System;
using Godot;

/// <summary>
/// Driver for the <see cref="RichTextLabel"/> control.
/// </summary>
/// <typeparam name="T">RichTextLabel type.</typeparam>
public class RichTextLabelDriver<T> : ControlDriver<T> where T : RichTextLabel
{
    /// <summary>
    /// Creates a new generic RichTextLabelDriver.
    /// </summary>
    /// <param name="producer">Producer that creates a RichTextLabel subclass.</param>
    /// <param name="description">Driver description.</param>
    public RichTextLabelDriver(Func<T> producer, string description = "") : base(producer, description)
    {
    }

    /// <summary>
    /// The current text of the label.
    /// </summary>
    public string Text => PresentRoot.Text;

    /// <summary>
    /// Returns true if the label has bbcode enabled.
    /// </summary>
    public bool IsBbcodeEnabled => PresentRoot.BbcodeEnabled;
}

/// <summary>
/// Driver for the <see cref="RichTextLabel"/> control.
/// </summary>
public sealed class RichTextLabelDriver : RichTextLabelDriver<RichTextLabel>
{
    /// <summary>
    /// Creates a new RichTextLabelDriver.
    /// </summary>
    /// <param name="producer">Producer that creates a RichTextLabel subclass.</param>
    /// <param name="description">Driver description.</param>
    public RichTextLabelDriver(Func<RichTextLabel> producer, string description = "") : base(producer, description)
    {
    }
}
