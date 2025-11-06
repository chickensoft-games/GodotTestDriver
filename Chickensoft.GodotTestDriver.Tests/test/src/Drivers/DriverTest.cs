namespace Chickensoft.GodotTestDriver.Tests.Drivers;

using System.Threading.Tasks;
using Chickensoft.GoDotTest;
using Godot;

public abstract class DriverTest : TestClass
{
  protected Fixture Fixture { get; }
  protected Node RootNode { get; private set; } = null!;

  protected DriverTest(Node testScene) : base(testScene)
  {
    Fixture = new Fixture(testScene.GetTree());
  }

  [Setup]
  public async Task Setup()
  {
    RootNode = await Fixture.LoadAndAddScene<Node>(
      $"res://test/src/Drivers/{GetType().Name}.tscn"
    );
  }

  [Cleanup]
  public async Task Cleanup() => await Fixture.Cleanup();
}
