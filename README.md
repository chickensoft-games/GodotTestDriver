# Godot Test Driver

[![Chickensoft Badge][chickensoft-badge]][chickensoft-website] [![Discord][discord-badge]][discord] [![Read the docs][read-the-docs-badge]][docs]

This library provides an API that simplifies writing integration tests for Godot projects.

- ✅ Decouple your integration tests from the implementation details of your Godot project.
- ✅ Easily simulate mouse clicks, keystrokes, and input actions.
- ✅ Drivers for many of Godot's built-in nodes, making it easy to simulate complex interactions.
- ✅ Support for easily setting up test fixtures and destroying them properly after the test.

> [!NOTE]
> GodotTestDriver was created by [derkork](https://github.com/derkork) and is now maintained (with permission) by the [Chickensoft](https://chickensoft.games) open source community.

---

<p align="center">
<img alt="Chickensoft.GodotTestDriver" src="Chickensoft.GodotTestDriver/icon.png" width="200">
</p>

---

> [!NOTE]
> GodotTestDriver is not a test executor. We recommend [GoDotTest] for executing tests inside your Godot game.

## How to use GodotTestDriver

### Installation

GodotTestDriver is published on [NuGet](https://www.nuget.org/packages/GodotTestDriver). To add it use this command line command (or the NuGet facilities of your IDE):

```bash
dotnet add package GodotTestDriver
```

If you are targeting `netstandard2.1` also add the following lines to your `.csproj` file to make it work with Godot:

```xml
<PropertyGroup>
    <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
</PropertyGroup>
```

### Real-world Examples

- [OpenSCAD Graph Editor](https://github.com/derkork/openscad-graph-editor/tree/master/Tests)
- [Chickensoft 3D Game Demo](https://github.com/chickensoft-games/GameDemo)

### Fixtures

This library provides a `Fixture` class which you can use to create and automatically dispose of Godot nodes and scenes. The fixture ensures that all tree modifications run on the main thread.

```csharp
using GodotTestDriver;

class MyTest
{
     // You will need get hold of a SceneTree instance. The way you get
     // hold of it will depend on the testing framework you use.
    SceneTree tree = ...;
    Fixture fixture;
    Player player;
    Arena arena;

    // This is a setup method. The exact way of how stuff is set up
    // differs from framework to framework, but most have a setup
    // method.
    async Task Setup()
    {
        // Create a new Fixture instance.
        fixture = new Fixture(tree);

        // load the arena scene. It will be automatically
        // disposed of when the fixture is disposed.
        arena = await fixture.LoadAndAddScene<Arena>("res://arena.tscn");

        // load the player. it also will be automatically disposed.
        player = fixture.LoadScene<Player>("res://player.tscn");

        // add the player to the arena.
        arena.AddChild(player);
    }


    async Task TestBattle()
    {
        // load a monster. again, it will be automatically disposed.
        var monster = fixture.LoadScene<Monster>("res://monster.tscn");

        // add the monster to the arena
        arena.AddChild(monster);

        // create a weapon on the fly without loading a scene.
        // We call fixture.AutoFree to schedule this object for
        // deletion when the fixture is cleaned up.
        var weapon = fixture.AutoFree(new Weapon());

        // add the weapon to the player.
        arena.AddChild(weapon);


        // run the actual tests.
        ....
    }

    // You can also add custom cleanup steps to the fixture while
    // the test is running. These will be performed after the
    // test is done. This is very useful for cleaning up stuff
    // that is created during the tests.
    async Task TestSaving()
    {
        ...
        // save the game
        GameDialog.SaveButton.Click();

        // await file operations here

        // instruct the fixture to delete our savegame in the
        // cleanup phase.
        fixture.AddCleanupStep(() => File.Delete("user://savegame.dat"));

        // assert that the game was saved
        Assert.That(File.Exists("user://savegame.dat"));

        ....
        // when the test is done, the fixture will run your custom
        // cleanup step (e.g. delete the save game in this case)
    }


    // This is a cleanup method. Like the setup method, the exact
    // way of how stuff is cleaned up differs from framework to
    // framework, but most have a cleanup method.
    async Task TearDown()
    {
        // dispose of anything we created during the test.
        // this will also run all custom cleanup steps.
        await Fixture.Cleanup();
    }
}
```

#### Loading scenes by naming convention

If you have many scenes in your project, it may become cumbersome to hard-code scene paths into your tests all the time. This will also make it harder to move scenes around in your project.

To solve this, you can make your scenes follow a naming convention. For example, say the root node of your `Player/Player.tscn` scene is the  `Player` node which has its script stored in `Player/Player.cs`.  You can then simply load the scene like this:

```cs
var player = fixture.LoadScene<Player>();
```

For this to work, it is important that the scene file and the script file have the same name, same spelling and casing and must reside in the same directory. The only difference must be the file extension - `.tscn` for the scene file and `.cs` for the script file.

## Test drivers

### Introduction

Test drivers serve as an abstraction layer between your test code and your game code. They are a high level interface through which the tests can "see" the game and interact with it. With a test driver, your game tests do not need to know how the game works under the hood. This makes your tests a lot more robust to change.

### Producing nodes for the test driver to work on

Test drivers work on a part of the node tree. Each test driver takes a _producer_ as argument, which is a function that is supposed to produce a node from the current tree that the driver will work on. E.g. the `ButtonDriver` takes a function that produces a button node.

How exactly this node is produced depends on your game and test setup. Lets say you would use a classic test framework that has some kind of `Setup` method:

```csharp
class MyTest
{
    ButtonDriver buttonDriver;

    async Task Setup()
    {
        buttonDriver = new ButtonDriver(() => GetTree().GetNodeOrNull<Button>("UI/MyButton"));

        // ... more setup here
    }
}
```

In this example, the `ButtonDriver` would try to get the node it should work on using the `GetNodeOrNull`  function. When the driver is constructed, it will not check whether the node is actually present. This only happens when the driver is used. This way you can set up a driver without having a matching node structure in place. This is very useful as node structures can dynamically change while your tests are running (e.g. a dialog can be added to the scene or removed from it, same with monsters or players).

### Using the test driver

After you have created the test driver you can use it in your tests:

```csharp

void TestButtonDisappearsWhenClicked()
{
    // when
    // will click the button in its center. This will actually
    // move the mouse set a click and trigger all the events of a
    // proper button click.
    buttonDriver.ClickCenter();

    // then
    // the button should be present but invisible.
    Assert.That(button.Visible).IsFalse();
}

```

Note how your tests now interface with the driver, rather than the underlying node structure. When the `ClickCenter` method is called and the button is not actually present and visible, the method will throw an exception explaining why you cannot click the button right now. This way you will get proper error messages when you are testing your game and not just `NullReferenceException`s which greatly helps in debugging tests.

### Composition of test drivers

Using a test driver by its own is nice, but it is only enough for very simple cases. Most of the time you will have complex nested node structures that make up your game entities and the UI. You can therefore compose test drivers into tree-like structures to represent these entities. Let's say you have a dialog popping up asking the player whether they want to save the game before quitting. It consists of three buttons and a label.

You can write a custom driver that represents this dialog to your tests:

```csharp

// the root of the dialog would be a panel container.
class ConfirmationDialogDriver : ControlDriver<PanelContainer>
{
    // we have a label and three buttons
    public LabelDriver Label { get; }
    public ButtonDriver YesButton { get; }
    public ButtonDriver NoButton { get; }
    public ButtonDriver CancelButton { get; }

    public ConfirmationDialogDriver(Func<PanelContainer> producer)
      : base(producer)
    {
        // for each of the elements we create a new driver, that
        // uses a producer fetching the respective node from below
        // our own root node.

        // Root is a built-in property of the driver base class,
        // which will run the producer function to get the root node.
        Label = new LabelDriver(() => Root?.GetNodeOrNull<Label>("VBox/Label"));
        YesButton = new ButtonDriver(() => Root?.GetNodeOrNull<Button>("VBox/HBox/YesButton"));
        NoButton = new ButtonDriver(() => Root?.GetNodeOrNull<Button>("VBox/HBox/NoButton"));
        CancelButton = new ButtonDriver(() => Root?.GetNodeOrNull<Button>("VBox/HBox/CancelButton"));
    }
}
```

Now we can use this driver in our tests to test the dialog:

```csharp
ConfirmationDialogDriver dialogDriver;

async Task Setup()
{
    // prepare the driver
    dialogDriver = new ConfirmationDialogDriver(() => GetTree().GetNodeOrNull<PanelContainer>("UI/ConfirmationDialog"));
}


void ClickingYesClosesTheDialog()
{
    // when
    // we click the yes button.
    dialogDriver.YesButton.ClickCenter();

    // then
    // the dialog should be gone.
    Assert.That(dialogDriver.Visible).IsFalse();
}
```

Note that because of the way drivers are implemented `dialogDriver.YesButton` will never throw a `NullReferenceException` even if the button is currently not present in the tree. This greatly simplifies your testing code. Also your testing code now is fully decoupled from the actual node structure. If you decide to change the node structure of the dialog, you will only need to change the `ConfirmationDialogDriver`, instead of all the tests that use it.

### Built-in drivers

- [BaseButtonDriver](GodotTestDriver/Drivers/BaseButtonDriver.cs) - a driver base class for button-like UI elements
- [ButtonDriver](GodotTestDriver/Drivers/ButtonDriver.cs) - a driver for buttons
- [Camera2DDriver](GodotTestDriver/Drivers/Camera2DDriver.cs) - a driver for 2D cameras
- [CanvasItemDriver](GodotTestDriver/Drivers/CanvasItemDriver.cs) - a driver for canvas items
- [CheckBoxDriver](GodotTestDriver/Drivers/CheckBoxDriver.cs) - a driver for check boxes
- [ControlDriver](GodotTestDriver/Drivers/ControlDriver.cs) - the root driver class for drivers working on controls, can be used for any control
- [GraphEditDriver](GodotTestDriver/Drivers/GraphEditDriver.cs) - a driver for graph editors
- [GraphNodeDriver](GodotTestDriver/Drivers/GraphNodeDriver.cs) - a driver for graph nodes
- [ItemListDriver](GodotTestDriver/Drivers/ItemListDriver.cs) - a driver for item lists
- [LabelDriver](GodotTestDriver/Drivers/LabelDriver.cs) - a driver for labels
- [LineEditDriver](GodotTestDriver/Drivers/LineEditDriver.cs) - a driver for line edits
- [Node2DDriver](GodotTestDriver/Drivers/Node2DDriver.cs) - a driver for 2D nodes
- [NodeDriver](GodotTestDriver/Drivers/NodeDriver.cs) - the root driver class.
- [OptionButtonDriver](GodotTestDriver/Drivers/OptionButtonDriver.cs) - a driver for option buttons
- [PopupMenuDriver](GodotTestDriver/Drivers/PopupMenuDriver.cs) - a driver for popup menus
- [RichTextLabelDriver](GodotTestDriver/Drivers/RichTextLabelDriver.cs) - a driver for rich text labels
- [Sprite2DDriver](GodotTestDriver/Drivers/Sprite2DDriver.cs) - a driver for 2D sprites
- [TextEditDriver](GodotTestDriver/Drivers/TextEditDriver.cs) - a driver for text edits
- [WindowDriver](GodotTestDriver/Drivers/WindowDriver.cs) - a driver for windows

## Input

GodotTestDriver provides a number of extension methods that allow you to simulate user input.

> [!NOTE]
> With the exception of methods that require time to elapse (e.g., `Node::HoldActionFor()`), these functions do not wait for additional frames to elapse. When you need additional frames to handle input, GodotTestDriver provides waiting extensions, described below.

### Simulating mouse input

GodotTest provides a number of extension functions on `Viewport` that allow you to simulate mouse input in a viewport.

```csharp

// you can move the mouse to a certain position (e.g. for simulating a hover)
viewport.MoveMouseTo(new Vector2(100, 100));

// you can click at a certain position (default is left mouse button)
viewport.ClickMouseAt(new Vector2(100, 100));

// you can give a ButtonList argument to click with a different mouse button
viewport.ClickMouseAt(new Vector2(100, 100), ButtonList.Right);

// you can also send single mouse presses and releases
viewport.PressMouse();
viewport.ReleaseMouse();

// there is also built-in support for mouse dragging
// this will press the mouse at the first point, then move it to the
// second point and release it there.
viewport.DragMouse(new Vector2(100, 100), new Vector2(400, 400));

// again you can give a ButtonList argument to drag with a different mouse button
viewport.DragMouse(new Vector2(100, 100), new Vector2(400, 400), ButtonList.Right);
```

### Simulating keyboard input

GodotTest provides a number of extension functions on `SceneTree`/`Node` that allow you to simulate keyboard input.

```csharp

// you can press down a key
node.PressKey(KeyList.A);
// you can also specify modifiers (e.g. shift+F1)
node.PressKey(KeyList.F1, shift: true);
// you can also specify multiple modifiers (e.g. ctrl+shift+F1)
node.PressKey(KeyList.F1, control: true, shift: true);

// you can release a key
node.ReleaseKey(KeyList.A);

// you can also combine pressing and releasing a key
node.TypeKey(KeyList.A);
```

### Simulating controller input

GodotTest provides a number of extension functions on `SceneTree`/`Node` that allow you to simulate controller input using Godot's [`InputEventJoypadButton`](https://docs.godotengine.org/en/stable/classes/class_inputeventjoypadbutton.html#class-inputeventjoypadbutton) and [`InputEventJoypadMotion`](https://docs.godotengine.org/en/stable/classes/class_inputeventjoypadmotion.html#class-inputeventjoypadmotion) events.

```csharp
// you can press down a controller button
node.PressJoypadButton(JoyButton.Y);

// you can release a controller button
node.ReleaseJoypadButton(JoyButton.Y);

// you can specify a particular controller device
var deviceID = 0;
node.PressJoypadButton(JoyButton.Y, deviceID);
node.ReleaseJoypadButton(JoyButton.Y, deviceID);

// you can combine pressing and releasing a button
node.TapJoypadButton(JoyButton.Y, deviceID);

// you can move an analog controller axis to a given position, with 0 being the rest position
// for instance:
// * a gamepad trigger will range from 0 to 1
// * a thumbstick's x-axis will range from -1 to 1
node.MoveJoypadAxisTo(JoyAxis.RightX, -0.3f);

// you can release a controller axis (equivalent to setting its position to 0)
node.ReleaseJoypadAxis(JoyAxis.RightX);

// you can specify a particular controller device
node.MoveJoypadAxisTo(JoyAxis.RightX, -0.3f, deviceID);
node.ReleaseJoypadAxis(JoyAxis.RightX, deviceID);

// hold a controller button for 1.5 seconds
await node.HoldJoypadButtonFor(1.5f, JoyButton.Y, deviceID);
// hold a controller axis position for 1.5 seconds
await node.HoldJoypadAxisFor(1.5f, JoyAxis.RightX, -0.3f, deviceID);
```

To simulate [controller input using mapped actions](https://docs.godotengine.org/en/stable/tutorials/inputs/controllers_gamepads_joysticks.html#which-input-singleton-method-should-i-use), for use with Godot's `Input.GetActionStrength()`, `Input.GetAxis()`, and `Input.GetVector()` methods, see the next section.

### Simulating other actions

Since version 2.1.0 you can now also simulate actions like this:

```csharp
// start the jump action
node.StartAction("jump");
// end the jump action
node.EndAction("jump");

// hold an action pressed for 1 second
await node.HoldActionFor(1.0f, "jump");
```

## Waiting extensions

GodotTestDriver provides a number of extension functions on `SceneTree` which allow you to wait for certain events to happen. This is a common requirement in integration tests, where you will click or send some key strokes and then some action happens that takes a while to process.

```csharp

Fixture fixture;
// this is a custom driver for the game under test
ArenaDriver arena;

public async Task Setup()
{
    fixture = new Fixture(GetTree());
    // add the arena to the scene
    var arenaInstance = fixture.LoadAndAddScene("res://arena.tscn");
    arena = new ArenaDriver(() => arenaInstance);

    // load a monster and put it into the arena
    var monster = fixture.LoadScene<Monster>("res://monster.tscn");
    arena.AddMonster(monster);

    // load a player and put it into the arena
    var player = fixture.LoadScene<Player>("res://player.tscn");
    arena.AddPlayer(player);
}

// you can wait for a certain amount of time for a condition
// to become true
public async Task TestCombat()
{
    // when
    // i open the arena gates
    arena.OpenGates();

    // then
    // within 5 seconds the player should be dead because
    // the monster will attack the player.
    await GetTree().WithinSeconds(5, () => {
        // this assertion will be repeatedly run every frame
        // until it either succeeds or the 5 seconds have elapsed
        Assert.True(arena.Player.IsDead);
    });
}

// you can also check for a condition to stay true for a
// certain amount of time
public async Task TestGodMode()
{
    // setup
    // give god mode to the player
    arena.Player.EnableGodMode();

    // when
    // i open the arena gates
    arena.OpenGates();

    // then
    // the player will not lose any health within the next 5 seconds
    await GetTree().DuringSeconds(5, () => {
        // this assertion will be repeatedly run every frame
        // until it either fails or the 5 seconds have elapsed
        Assert.Equal(arenaDriver.Player.MaxHealth, arenaDriver.Player.Health);
    });
}

```

### Writing Your Own Drivers

- All calls should succeed if the controlled object is in a suitable state to perform the requested operation. Otherwise, these calls should throw an `InvalidOperationException`. For example if you use a `ButtonDriver` and the button is not currently visible when you try to click it, the driver will throw an `InvalidOperationException`.
- Producer functions should never throw an exception. If they cannot find the node, they should just return `null`.

[GoDotTest]: https://github.com/chickensoft-games/GoDotTest
[chickensoft-badge]: https://chickensoft.games/img/badges/chickensoft_badge.svg
[chickensoft-website]: https://chickensoft.games
[discord-badge]: https://chickensoft.games/img/badges/discord_badge.svg
[discord]: https://discord.gg/gSjaPgMmYW
[read-the-docs-badge]: https://chickensoft.games/img/badges/read_the_docs_badge.svg
[docs]: https://chickensoft.games/docs
