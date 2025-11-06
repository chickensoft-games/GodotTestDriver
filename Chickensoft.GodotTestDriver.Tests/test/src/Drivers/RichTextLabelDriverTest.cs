namespace Chickensoft.GodotTestDriver.Tests.Drivers;

using Chickensoft.GoDotTest;
using Godot;
using GodotTestDriver.Drivers;
using Shouldly;

public class RichTextLabelDriverTest : DriverTest
{
  private readonly RichTextLabelDriver _richTextLabel;
  private readonly RichTextLabelDriver _bbCodeRichTextLabel;

  public RichTextLabelDriverTest(Node testScene) : base(testScene)
  {
    _richTextLabel = new RichTextLabelDriver(
      () => RootNode.GetNode<RichTextLabel>("RichTextLabel")
    );
    _bbCodeRichTextLabel = new RichTextLabelDriver(
      () => RootNode.GetNode<RichTextLabel>("BbCodeRichTextLabel")
    );
  }

  [Test]
  public void InspectionWorks()
  {
    // WHEN
    // everything is set up

    // THEN
    // the rich text label has "Hello World!" as text and bbcode is disabled
    _richTextLabel.Text.ShouldBe("Hello World!");
    _richTextLabel.IsBbcodeEnabled.ShouldBeFalse();

    // and the bbcode rich text label has "Hello [b]World![/b]" as text and
    // bbcode is enabled
    _bbCodeRichTextLabel.Text.ShouldBe("Hello [b]World![/b]");
    _bbCodeRichTextLabel.IsBbcodeEnabled.ShouldBeTrue();
  }
}
