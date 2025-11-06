namespace Chickensoft.GodotTestDriver.Input;

using Godot;

/// <summary>
/// Extensions for simulating mouse inputs.
/// </summary>
public static class MouseInputExtensions
{
  /// <summary>
  /// Clicks the mouse at the specified position.
  /// </summary>
  /// <param name="viewport">Viewport that generates simulated input.</param>
  /// <param name="position">Position, in viewport coordinates.</param>
  /// <param name="button">Mouse button.</param>
  public static void ClickMouseAt(
    this Viewport viewport,
    Vector2 position,
    MouseButton button = MouseButton.Left
  )
  {
    viewport.PressMouseAt(position, button);
    viewport.ReleaseMouseAt(position, button);
  }

  /// <summary>
  /// Moves the mouse to the specified position.
  /// </summary>
  /// <param name="viewport">Viewport that generates simulated input.</param>
  /// <param name="position">Position, in viewport coordinates.</param>
  public static void MoveMouseTo(this Viewport viewport, Vector2 position)
  {
    var oldPosition = viewport.GetMousePosition();
    var stretchedPosition = viewport.StretchedPosition(position);

    viewport.WarpMouse(position);
    var inputEvent = new InputEventMouseMotion
    {
      GlobalPosition = stretchedPosition,
      Position = stretchedPosition,
      Relative = position - oldPosition
    };
    Input.ParseInputEvent(inputEvent);
    Input.FlushBufferedEvents();
  }

  /// <summary>
  /// Drags the mouse from the start position to the end position.
  /// </summary>
  /// <param name="viewport">Viewport that generates simulated input.</param>
  /// <param name="start">Start position, in viewport coordinates.</param>
  /// <param name="end">End position, in viewport coordinates.</param>
  /// <param name="button">Mouse button.</param>
  public static void DragMouse(
    this Viewport viewport,
    Vector2 start,
    Vector2 end,
    MouseButton button = MouseButton.Left
  )
  {
    viewport.PressMouseAt(start, button);
    viewport.ReleaseMouseAt(end, button);
  }

  /// <summary>
  /// Presses the given mouse button.
  /// </summary>
  /// <param name="_">Viewport that generates simulated input.</param>
  /// <param name="button">Mouse button (left by default).</param>
  public static void PressMouse(
    this Viewport _,
    MouseButton button = MouseButton.Left
  )
  {
    var action = new InputEventMouseButton
    {
      ButtonIndex = button,
      Pressed = true
    };
    Input.ParseInputEvent(action);
    Input.FlushBufferedEvents();
  }

  /// <summary>
  /// Releases the given mouse button.
  /// </summary>
  /// <param name="_">Viewport that generates simulated input.</param>
  /// <param name="button">Mouse button (left by default).</param>
  public static void ReleaseMouse(
    this Viewport _,
    MouseButton button = MouseButton.Left
  )
  {
    var action = new InputEventMouseButton
    {
      ButtonIndex = button,
      Pressed = false
    };
    Input.ParseInputEvent(action);
    Input.FlushBufferedEvents();
  }

  private static void PressMouseAt(
    this Viewport viewport,
    Vector2 position,
    MouseButton button = MouseButton.Left
  )
  {
    viewport.MoveMouseTo(position);
    var stretchedPosition = viewport.StretchedPosition(position);

    var action = new InputEventMouseButton
    {
      ButtonIndex = button,
      Pressed = true,
      Position = stretchedPosition,
      GlobalPosition = stretchedPosition
    };
    Input.ParseInputEvent(action);
    Input.FlushBufferedEvents();
  }

  private static void ReleaseMouseAt(
    this Viewport viewport,
    Vector2 position,
    MouseButton button = MouseButton.Left
  )
  {
    viewport.MoveMouseTo(position);
    var stretchedPosition = viewport.StretchedPosition(position);

    var action = new InputEventMouseButton
    {
      ButtonIndex = button,
      Pressed = false,
      Position = stretchedPosition,
      GlobalPosition = stretchedPosition
    };
    Input.ParseInputEvent(action);
    Input.FlushBufferedEvents();
  }

  // Correct for viewport's stretch transform
  // see https://docs.godotengine.org/en/stable/tutorials/2d/2d_transforms.html#stretch-transform
  private static Vector2 StretchedPosition(
    this Viewport viewport,
    Vector2 position
  )
  {
    var screenTransform = viewport.GetFinalTransform();
    return screenTransform.BasisXform(position) + screenTransform.Origin;
  }
}
