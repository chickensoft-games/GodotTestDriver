namespace Chickensoft.GodotTestDriver.Drivers;

using System;
using Godot;
using GodotTestDriver.Util;

/// <summary>
/// Driver for <see cref="Sprite2D"/> nodes.
/// </summary>
/// <typeparam name="T">Sprite2D type.</typeparam>
/// [PublicAPI]
public class Sprite2DDriver<T> : Node2DDriver<T> where T : Sprite2D
{
  /// <summary>
  /// Creates a new generic Sprite2DDriver.
  /// </summary>
  /// <param name="producer">Producer that creates a Sprite2D subclass.</param>
  /// <param name="description">Driver description.</param>
  public Sprite2DDriver(Func<T> producer, string description = "")
    : base(producer, description)
  {
  }

  /// <summary>
  /// Whether the sprite is flipped horizontally.
  /// </summary>
  public bool IsFlippedHorizontally => PresentRoot.FlipH;

  /// <summary>
  /// Whether the sprite is flipped vertically.
  /// </summary>
  public bool IsFlippedVertically => PresentRoot.FlipV;

  /// <summary>
  /// Returns true if the Node is visible and fully inside the viewport rect.
  /// </summary>
  public bool IsFullyInView
  {
    get
    {
      if (!IsVisible)
      {
        return false;
      }

      var sprite = VisibleRoot;
      var screenRect = sprite
        .GetViewport()
        .WorldToScreen(sprite.GetRect().ToGlobalRect(sprite.GlobalPosition));
      return sprite.GetViewportRect().Encloses(screenRect);
    }
  }
}

/// <summary>
/// Driver for <see cref="Sprite2D"/> nodes.
/// </summary>
public class Sprite2DDriver : Sprite2DDriver<Sprite2D>
{
  /// <summary>
  /// Creates a new Sprite2DDriver.
  /// </summary>
  /// <param name="producer">Producer that creates a Sprite2D subclass.</param>
  /// <param name="description">Driver description.</param>
  public Sprite2DDriver(Func<Sprite2D> producer, string description = "")
    : base(producer, description)
  {
  }
}
