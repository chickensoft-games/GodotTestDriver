namespace Chickensoft.GodotTestDriver.Tests;

using System;
using System.Threading.Tasks;
using Chickensoft.GoDotTest;
using Godot;
using GodotTestDriver.Drivers;
using JetBrains.Annotations;
using Shouldly;

[UsedImplicitly]
public class ButtonDriverTest : DriverTest
{
    private readonly ButtonDriver _button;
    private readonly LabelDriver _label;
    private readonly ControlDriver<Panel> _panel;

    public ButtonDriverTest(Node testScene) : base(testScene)
    {
        _button = new ButtonDriver(() => RootNode.GetNode<Button>("Button"));
        _label = new LabelDriver(() => RootNode.GetNode<Label>("Label"));
        _panel = new ControlDriver<Panel>(() => RootNode.GetNode<Panel>("Panel"));
    }

    [Test]
    public async Task ClickingWorks()
    {
        // WHEN
        // i click the button
        await _button.ClickCenter();
        // the label text changes.
        _label.Text.ShouldBe("did work");
        // and the panel disappears
        _panel.IsVisible.ShouldBeFalse();
    }

    [Test]
    public async Task ClickingDisabledButtonThrowsException()
    {
        // SETUP
        _button.PresentRoot.Disabled = true;
        // WHEN
        // i click the button then an exception is thrown
        await Should.ThrowAsync<InvalidOperationException>(async () => await _button.ClickCenter());
    }

    [Test]
    public async Task ClickingHiddenButtonThrowsException()
    {
        // SETUP
        _button.PresentRoot.Visible = false;
        // WHEN
        // i click the button then an exception is thrown
        await Should.ThrowAsync<InvalidOperationException>(async () => await _button.ClickCenter());
    }
}
