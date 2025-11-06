namespace Chickensoft.GodotTestDriver.Drivers;

using System;
using Godot;
using GodotTestDriver.Input;

/// <summary>
/// A driver for UI Windows.
/// </summary>
/// <typeparam name="T">Window type.</typeparam>
public class WindowDriver<T> : NodeDriver<T> where T : Window
{
  /// <summary>
  /// Creates a new generic WindowDriver.
  /// </summary>
  /// <param name="producer">Producer that creates a Window subclass.</param>
  /// <param name="description">Driver description.</param>
  public WindowDriver(Func<T> producer, string description = "")
    : base(producer, description)
  {
  }

  /// <summary>
  /// Is the window currently visible?
  /// </summary>
  public bool IsVisible => PresentRoot.Visible;

  /// <summary>
  /// The viewport that this window item is rendering to.
  /// </summary>
  public Viewport Viewport => PresentRoot.GetViewport();

  /// <summary>
  /// The position of the window in pixels.
  /// </summary>
  public Vector2I Position => PresentRoot.Position;

  /// <summary>
  /// Returns the root node but ensures it is visible.
  /// </summary>
  /// <exception cref="InvalidOperationException"/>
  protected T VisibleRoot
  {
    get
    {
      var root = PresentRoot;
      if (!root.Visible)
      {
        throw new InvalidOperationException(
          ErrorMessage("Cannot interact with window because it is not visible.")
        );
      }
      return root;
    }
  }

  /// <summary>
  /// Closes the window.
  /// </summary>
  public void Close()
  {
    var window = VisibleRoot;
    window.EmitSignal(Window.SignalName.CloseRequested);
  }

  /// <summary>
  /// Drags the window by the given offset.
  /// </summary>
  /// <param name="x">Horizontal cartesian coordinate component.</param>
  /// <param name="y">Vertical cartesian coordinate component.</param>
  public void DragByPixels(int x, int y) => DragByPixels(new Vector2I(x, y));

  /// <summary>
  /// Drags the window by the given offset.
  /// </summary>
  /// <param name="offset">Offset vector.</param>
  /// <exception cref="InvalidOperationException"/>
  public void DragByPixels(Vector2I offset)
  {
    var window = VisibleRoot;

    // check that the window has a parent otherwise we can't drag it
    if (window.GetParent() == null)
    {
      throw new InvalidOperationException(
        ErrorMessage("Dragging of root windows is not supported.")
      );
    }

    var titleBarHeight = window.GetThemeConstant("title_height");

    // get position and width of window, then use the title bar height to get
    // the center of the title bar note that the title bar is ABOVE the window,
    // so we need to subtract the title bar height from the window's position to
    // get the top of the title bar
    var pos = window.Position;
    var width = window.Size.X;
    var startSpot = new Vector2(
      pos.X + (width / 2f),
      pos.Y - (titleBarHeight / 2f)
    );
    var endSpot = startSpot + offset;

    window.GetParent().GetViewport().DragMouse(startSpot, endSpot);
  }
}

/// <summary>
/// A driver for UI Windows.
/// </summary>
public class WindowDriver : WindowDriver<Window>
{
  /// <summary>
  /// Creates a new WindowDriver.
  /// </summary>
  /// <param name="producer">Producer that creates a Window subclass.</param>
  /// <param name="description">Driver description.</param>
  public WindowDriver(Func<Window> producer, string description = "")
    : base(producer, description)
  {
  }
}
