namespace Chickensoft.GodotTestDriver.Drivers;

using System;
using Godot;
using JetBrains.Annotations;

/// <summary>
/// Driver for <see cref="Button"/> controls.
/// </summary>
/// <typeparam name="T">Button type.</typeparam>
[PublicAPI]
public class ButtonDriver<T> : BaseButtonDriver<T> where T : Button
{
    /// <summary>
    /// Creates a new generic ButtonDriver.
    /// </summary>
    /// <param name="producer">Producer that creates a Button subclass.</param>
    /// <param name="description">Driver description.</param>
    public ButtonDriver(Func<T> producer, string description = "") : base(producer, description)
    {
    }
}

/// <summary>
/// Driver for <see cref="Button"/> controls.
/// </summary>
[PublicAPI]
public sealed class ButtonDriver : ButtonDriver<Button>
{
    /// <summary>
    /// Creates a new ButtonDriver.
    /// </summary>
    /// <param name="producer">Producer that creates a Button subclass.</param>
    /// <param name="description">Driver description.</param>
    public ButtonDriver(Func<Button> producer, string description = "") : base(producer, description)
    {
    }
}
