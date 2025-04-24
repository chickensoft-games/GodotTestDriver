namespace Chickensoft.GodotTestDriver.Tests.Drivers;

using Chickensoft.GoDotTest;
using Godot;
using GodotTestDriver.Drivers;
using JetBrains.Annotations;
using Shouldly;

[UsedImplicitly]
public class CheckBoxDriverTest : DriverTest
{
    private readonly CheckBoxDriver _checkBox;

    public CheckBoxDriverTest(Node testScene) : base(testScene)
    {
        _checkBox = new CheckBoxDriver(() => RootNode.GetNode<CheckBox>("CheckBox"));
    }

    [Test]
    public void ClickingChecksAndUnchecks()
    {
        // WHEN
        // i click the checkbox
        _checkBox.ClickCenter();

        // THEN
        // the checkbox is checked
        _checkBox.IsChecked.ShouldBeTrue();

        // WHEN
        // i click the checkbox again
        _checkBox.ClickCenter();

        // THEN
        // the checkbox is unchecked
        _checkBox.IsChecked.ShouldBeFalse();
    }
}
