# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.1.0] - 2023-05-30

### Added

- New methods in `ActionsControlExtensions` allows triggering Godot input actions: `StartAction`, `EndAction`, `HoldActionFor`, and `TriggerAction`.
- A new method, `KeyboardControlExtensions.HoldKeyFor`, simulates a key combination being held for a certain amount of time.
- New `WaitExtensions.WaitUntil` method enables waiting until a predicate function returns true, or a configurable timeout occurs.
- New `WaitExtensions.Wait` method enables processing frames for a given length of time.
- New `WaitExtensions.WaitPhysics` enables processing physics frames for a given length of time.

### Fixed

- All node extension methods in `WaitExtensions` now require the node to be in the scene tree.

## [2.0.0] - 2023-03-05

## [2.0.0-pre2] - 2023-03-20

### Breaking Changes

- The `Fixture` API has been updated to provide a more consistent API interface.  All functions that create nodes  now have an optional `autoFree` parameter that enqueues the created node for auto-freeing on fixture cleanup. This parameter is `true` by default. Before some functions would free the nodes, others would not making it hard to predict when a node would be freed.
- The `AddToRoot` function of the `Fixture` class now has a parameter `autoRemoveFromRoot` that controls whether the node is removed from the root node on fixture cleanup. This parameter is `true` by default. Before the node was not automatically removed from the root node on fixture cleanup and you had to either remove or free it yourself.

### Added
- The `Fixture` class now can instantiate a scene by naming convention. E.g. if you have a script `Player/Player.cs` and a scene `Player/Player.tscn` next to it, you can now instantiate the scene by calling `fixture.LoadScene<Player>()`. This makes your tests more robust to changes in the scene structure as you don't have to update the path in your test code. Thanks to @definitelyokay for providing a PR for this feature!

- The tests can now run on GitHub using an emulated display driver. You can check the [workflow file](.github/workflows/visual_tests.yaml) for an example on how to set this up for your own project. Another big thanks to @definitelyokay for figuring this out and providing a PR for it!
- If you want to contribute to Godot Test Driver and use Visual Studio Code there are now a few launch configurations in the [.vscode](.vscode) folder that make it easier to debug the tests. Thanks to @definitelyokay for providing a PR for this as well!

## [2.0.0-pre1] - 2023-03-05

### Breaking Changes

- Version 2.0.0 has been updated for Godot 4. It is no longer compatible with Godot 3.
- .net targets have been updated to match Godot 4. Currently only `net6.0` is supported.
- `TweenDriver` has been removed as `Tween` is no longer a node.
- `EnterText` in `LineEditDriver` has been renamed to `SubmitText` to match the new name of the signal in Godot 4.
- `SelectableMenuItems` in `PopupMenuDriver` has been renamed to `SelectableItems` to match the naming convention of other drivers.
- `MenuItems` in `PopupMenuDriver` has been renamed to `Items` to match the naming convention of other drivers.
- The experimental screenshot extension has been removed. It may be re-added in a future version but for now it's not useful the way it is.
- `BBCodeText` in `RichTextLabelDriver` has been removed as in Godot 4 `RichTextLabel` no longer has a separate property for BBCode text.

### Added

- A new `WindowDriver` class has been added to use as base class for driving `Window` nodes.
- A new `Sprite2DDriver` class has been added to use as base class for driving `Sprite2D` nodes.
- `GraphNodeDriver` now has  `Rect` property to get the node's rect.
- All node drivers have now a `GetSignalAwaiter` method to allow awaiting signals that the underlying node emits. Note, that you need to get the awaiter _before_ you run the code that emits the signal. Otherwise the awaiter will never complete.
- `ItemListDriver` now has an `ItemCount` property to get the number of items in the list.
- `ItemListDriver` has a new function `SelectItemsWithText` which allows to select multiple items at once.
- `ItemListDriver`s `SelectItemWithText` now has an optional parameter `addToSelection` to allow adding to the current selection instead of replacing it.
- `ItemListDriver` now has a `DeselectAll` function to deselect all items.
- `ItemListDriver` now has an `ActivateItemWithText` function to activate (double click or press enter on) an item by its text.
- `RichTextLabelDriver` now has a `IsBBCodeEnabled` property to check whether BBCode is enabled.

### Fixed

- `IsFullyInView` in `ControlDriver` now works correctly when the canvas transform is translated or scaled.
- `SelectableItems` in `ItemListDriver` now no longer returns items that are disabled.


## [1.0.0] - 2022-07-14

### Breaking Changes

- `GraphEditDriver` will now throw an exception when trying to get connections from it and the underlying `GraphEdit` does not exist in the tree. Previously it would return an empty list of connections. This is to make it consistent with the behavior of the other drivers.

### Added

- `GraphEditDriver` now has two new methods `HasConnectionFrom` and `HasConnectionTo` which allow you to check whether a certain port is connected without having to know which other port it is connected to.

## [0.1.0] - 2022-07-08

### Added

- Support for multiple .net targets. Now targets `net472` and `netstandard2.1`.
- `GraphNodeDriver` now has a function `GetPortType` to to inspect the node's port types.

## [0.0.30] - 2022-06-24

- Initial public release
