namespace Chickensoft.GodotTestDriver.Drivers;

using System;
using Godot;

/// <summary>
/// Driver for <see cref="CanvasItem"/> nodes.
/// </summary>
/// <typeparam name="T">CanvasItem type.</typeparam>
public class CanvasItemDriver<T> : NodeDriver<T> where T : CanvasItem
{
    /// <summary>
    /// Creates a new generic CanvasItemDriver.
    /// </summary>
    /// <param name="producer">Producer that creates a CanvasItem subclass.</param>
    /// <param name="description">Driver description.</param>
    public CanvasItemDriver(Func<T> producer, string description = "") : base(producer, description)
    {
    }

    /// <summary>
    /// Is the CanvasItem currently visible?
    /// </summary>
    public bool IsVisible => PresentRoot.IsVisibleInTree();

    /// <summary>
    /// The viewport that this canvas item is rendering to.
    /// </summary>
    public Viewport Viewport => PresentRoot.GetViewport();

    /// <summary>
    /// Returns the root node but ensures it is visible.
    /// </summary>
    /// <exception cref="InvalidOperationException"/>
    protected T VisibleRoot
    {
        get
        {
            var root = PresentRoot;
            return !root.IsVisibleInTree()
                      ? throw new InvalidOperationException(ErrorMessage("Cannot interact with CanvasItem because it is not visible."))
                      : root;
        }
    }
}
