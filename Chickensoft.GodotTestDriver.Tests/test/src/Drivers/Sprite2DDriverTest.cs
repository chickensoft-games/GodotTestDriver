namespace Chickensoft.GodotTestDriver.Tests.Drivers;

using Chickensoft.GoDotTest;
using Godot;
using GodotTestDriver.Drivers;
using Shouldly;

public class Sprite2DDriverTest : DriverTest
{
  private readonly Sprite2DDriver _sprite2D;
  private readonly Sprite2DDriver _partiallyVisibleSprite2D;

  public Sprite2DDriverTest(Node testScene) : base(testScene)
  {
    _sprite2D = new Sprite2DDriver(
      () => RootNode.GetNode<Sprite2D>("Sprite2D")
    );
    _partiallyVisibleSprite2D = new Sprite2DDriver(
      () => RootNode.GetNode<Sprite2D>("PartiallyVisibleSprite2D")
    );
  }

  [Test]
  public void InspectionWorks()
  {
    // WHEN
    // everything is set up

    // THEN
    // the sprite is visible
    _sprite2D.IsVisible.ShouldBeTrue();
    // it is not flipped horizontally but it is flipped vertically
    _sprite2D.IsFlippedHorizontally.ShouldBeFalse();
    _sprite2D.IsFlippedVertically.ShouldBeTrue();
    // and is fully in view
    _sprite2D.IsFullyInView.ShouldBeTrue();

    // and the partially visible sprite is visible
    _partiallyVisibleSprite2D.IsVisible.ShouldBeTrue();
    // is flipped horizontally but not vertically
    _partiallyVisibleSprite2D.IsFlippedHorizontally.ShouldBeTrue();
    _partiallyVisibleSprite2D.IsFlippedVertically.ShouldBeFalse();

    // and is not fully in view
    _partiallyVisibleSprite2D.IsFullyInView.ShouldBeFalse();
  }
}
