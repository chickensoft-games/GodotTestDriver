namespace Chickensoft.GodotTestDriver.Drivers;

using System;
using Godot;

/// <summary>
/// This structure identifies a port of a <see cref="GraphNode"/>
/// </summary>
public readonly struct Port
{
    /// <summary>
    /// The index of the port.
    /// </summary>
    public int PortIndex { get; }

    private readonly bool _isInput;

    /// <summary>
    /// Whether this port is an input port.
    /// </summary>
    public bool IsInput => IsDefined && _isInput;

    /// <summary>
    /// Whether this port is an output port.
    /// </summary>
    public bool IsOutput => IsDefined && !_isInput;

    /// <summary>
    /// The default value of this is false, so if you do a Port foo = default you will get an undefined port id.
    /// </summary>
    public bool IsDefined { get; }

    private Port(int port, bool isInput)
    {
        if (port < 0)
        {
            throw new ArgumentException("Port index must be greater than or equal to zero.");
        }
        PortIndex = port;
        _isInput = isInput;
        IsDefined = true;
    }

    /// <summary>
    /// Creates a new input port.
    /// </summary>
    /// <param name="port">Port index.</param>
    /// <returns>A new port instance.</returns>
    public static Port Input(int port)
    {
        return new(port, true);
    }

    /// <summary>
    /// Creates a new output port.
    /// </summary>
    /// <param name="port">Port index.</param>
    /// <returns>A new port instance.</returns>
    public static Port Output(int port)
    {
        return new(port, false);
    }

    /// <summary>
    /// The default Port is undefined and represents no port.
    /// </summary>
    public static readonly Port None;

    /// <summary>
    /// String representation of the port.
    /// </summary>
    public override string ToString()
    {
        return IsInput ? $"Input Port {PortIndex}" : $"Output Port {PortIndex}";
    }
}
