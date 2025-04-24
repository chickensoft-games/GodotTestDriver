namespace Chickensoft.GodotTestDriver.Drivers;

using System;
using Godot;

/// <summary>
/// Driver for a <see cref="CheckBox"/>.
/// </summary>
/// <typeparam name="T">CheckBox type.</typeparam>
public class CheckBoxDriver<T> : ButtonDriver<T> where T : CheckBox
{
    /// <summary>
    /// Creates a new generic CheckBoxDriver.
    /// </summary>
    /// <param name="producer">Producer that creates a CheckBox subclass.</param>
    /// <param name="description">Driver description.</param>
    public CheckBoxDriver(Func<T> producer, string description = "") : base(producer, description)
    {
    }

    /// <summary>
    /// Whether the checkbox is currently checked.
    /// </summary>
    public bool IsChecked => PresentRoot.ButtonPressed;
}

/// <summary>
/// Driver for a <see cref="CheckBox"/>.
/// </summary>
public sealed class CheckBoxDriver : CheckBoxDriver<CheckBox>
{
    /// <summary>
    /// Creates a new CheckBoxDriver.
    /// </summary>
    /// <param name="producer">Producer that creates a CheckBox subclass.</param>
    /// <param name="description">Driver description.</param>
    public CheckBoxDriver(Func<CheckBox> producer, string description = "") : base(producer, description)
    {
    }
}
