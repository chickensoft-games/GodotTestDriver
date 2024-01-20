namespace Chickensoft.GodotTestDriver.Tests;

using System.Threading.Tasks;
using Chickensoft.GoDotTest;
using Godot;
using GodotTestDriver.Drivers;
using Shouldly;

public class WindowDriverTest : DriverTest
{
    private readonly WindowDriver _window;

    public WindowDriverTest(Node testScene) : base(testScene)
    {
        _window = new WindowDriver(() => RootNode.GetNode<Window>("Window"));
    }

    [Test]
    public async Task WindowClosingWorks()
    {
        // GIVEN
        // the window is visible
        _window.IsVisible.ShouldBeTrue();

        // WHEN
        // i close the window
        await _window.Close();

        // THEN
        // the window is not visible
        _window.IsVisible.ShouldBeFalse();
    }

    /// <summary>
    /// dragging the window works
    /// </summary>
    [Test]
    public async Task WindowDraggingWorks()
    {
        var initialPosition = _window.Position;
        // WHEN
        // i drag the window
        await _window.DragByPixels(100, 100);

        // THEN
        // the window is visible
        _window.IsVisible.ShouldBeTrue();
        // and the window is at the correct position
        _window.Position.ShouldBe(initialPosition + new Vector2I(100, 100));
    }
}
