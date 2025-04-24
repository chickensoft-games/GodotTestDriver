namespace Chickensoft.GodotTestDriver.Tests.Drivers;

using System.Threading.Tasks;
using Chickensoft.GoDotTest;
using Godot;
using GodotTestDriver.Drivers;
using JetBrains.Annotations;
using Shouldly;

[UsedImplicitly]
public class Camera2DDriverTest : DriverTest
{
    private readonly Camera2DDriver _camera2D;
    private readonly Sprite2DDriver _centerSprite;
    private readonly Sprite2DDriver _offCenterSprite;

    public Camera2DDriverTest(Node testScene) : base(testScene)
    {
        _camera2D = new Camera2DDriver(() => RootNode.GetNode<Camera2D>("Camera2D"));
        _centerSprite = new Sprite2DDriver(() => RootNode.GetNode<Sprite2D>("CenterSprite"));
        _offCenterSprite = new Sprite2DDriver(() => RootNode.GetNode<Sprite2D>("OffCenterSprite"));
    }

    [Test]
    public void Camera2DIsPresent()
    {
        _camera2D.IsPresent.ShouldBeTrue();
        //  and center sprite is visible
        _centerSprite.IsFullyInView.ShouldBeTrue();
        //  and off center sprite is not visible
        _offCenterSprite.IsFullyInView.ShouldBeFalse();
    }

    [Test]
    public async Task Camera2DCanMove()
    {
        // WHEN
        // I move camera to off-center sprite
        var moveTask = _camera2D.MoveIntoView(_offCenterSprite.GlobalPosition, 2);

        // THEN
        //  the camera has successfully moved to the new position
        (await moveTask).ShouldBeTrue();

        //  and off center sprite is visible
        _offCenterSprite.IsFullyInView.ShouldBeTrue();
    }
}
