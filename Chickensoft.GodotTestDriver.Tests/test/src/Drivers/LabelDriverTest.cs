namespace Chickensoft.GodotTestDriver.Tests.Drivers;

using Chickensoft.GoDotTest;
using Godot;
using GodotTestDriver.Drivers;
using Shouldly;

public class LabelDriverTest : DriverTest
{
  private readonly LabelDriver _label;

  public LabelDriverTest(Node testScene) : base(testScene)
  {
    _label = new LabelDriver(() => RootNode.GetNode<Label>("Label"));
  }

  [Test]
  public void InspectionWorks() =>
    // WHEN
    // everything is set up

    // THEN
    // the label text is "Hello World!"
    _label.Text.ShouldBe("Hello World!");
}
