namespace Chickensoft.GodotTestDriver;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Godot;
using GodotTestDriver.Util;
using JetBrains.Annotations;
using Object = Godot.GodotObject;

/// <summary>
/// This is a test fixture that can be use to manage nodes in the test scene. It ensures that modifications to the scene are
/// always executed in the main thread. It also provides a simple means of cleaning up the scene after each test.
/// </summary>
[PublicAPI]
public class Fixture
{
    private readonly List<Func<Task>> _cleanupSteps = new();

    /// <summary>
    /// The <see cref="SceneTree"/> that this fixture is managing.
    /// </summary>
    public SceneTree Tree { get; }

    /// <summary>
    /// Create a new fixture for use with a scene tree.
    /// </summary>
    /// <param name="tree">Godot scene tree.</param>
    public Fixture(SceneTree tree)
    {
        Tree = tree;
    }

    /// <summary>
    /// Adds a node to the root of the tree. When this call returns the node will be in the scene and its
    /// <see cref="Node._Ready"/> function will have been called by Godot.
    /// </summary>
    /// <typeparam name="T">Scene's root node type.</typeparam>
    /// <param name="node">the node to be added to the tree root</param>
    /// <param name="autoRemoveFromRoot">if set to true, the node will be automatically removed from the root, when
    /// fixture's <see cref="Cleanup" /> method is called. Note that it will _not_ be freed.</param>
    public async Task<T> AddToRoot<T>(T node, bool autoRemoveFromRoot = true) where T : Node
    {
        Tree.Root.AddChild(node);

        if (autoRemoveFromRoot)
        {
            AddCleanupStep(() =>
            {
                if (Object.IsInstanceValid(node) && Object.IsInstanceValid(node.GetParent()))
                {
                    node.GetParent().RemoveChild(node);
                }
            });
        }

        await Tree.NextFrame(); // make sure _Ready is called.
        return node;
    }

    /// <summary>
    /// Loads a scene from the given path and adds it to the root of the tree.
    /// </summary>
    /// <typeparam name="T">Scene's root node type.</typeparam>
    /// <param name="path">the path to the scene file that should be loaded</param>
    /// <param name="autoFree">if set to true, the instance will be automatically freed when the fixture's
    /// <see cref="Cleanup" /> method is called</param>
    /// <param name="autoRemoveFromRoot">if set to true, the scene  will be automatically removed from the root, when
    /// fixture's <see cref="Cleanup" /> method is called. Note that it will _not_ be freed automatically unless
    /// <paramref name="autoFree"/> is also set to true.</param>
    public async Task<T> LoadAndAddScene<T>(string path, bool autoFree = true, bool autoRemoveFromRoot = true) where T : Node
    {
        var instance = LoadScene<T>(path, autoFree);
        return await AddToRoot(instance, !autoFree && autoRemoveFromRoot);
    }

    /// <summary>
    /// Loads and instantiates the scene that corresponds to the given script type ands adds it to the root of the tree.
    /// </summary>
    /// <typeparam name="T">Scene's root node type.</typeparam>
    /// <param name="autoFree">if set to true, the instance will be automatically freed when the fixture's
    /// <see cref="Cleanup" /> method is called</param>
    /// <param name="autoRemoveFromRoot">if set to true, the scene  will be automatically removed from the root, when
    /// fixture's <see cref="Cleanup" /> method is called. Note that it will _not_ be freed automatically unless
    /// <paramref name="autoFree"/> is also set to true.</param>
    public async Task<T> LoadAndAddScene<T>(bool autoFree = true, bool autoRemoveFromRoot = true) where T : Node
    {
        var instance = LoadScene<T>(autoFree);
        return await AddToRoot(instance, !autoFree && autoRemoveFromRoot);
    }

    /// <summary>
    /// Loads and instantiates the scene that corresponds to the given script type. The scene must
    /// be in the same directory, have the same name, and end with ".tscn".
    /// </summary>
    /// <param name="autoFree">if set to true, the instance will be automatically freed when the fixture's
    /// <see cref="Cleanup" /> method is called</param>
    /// <typeparam name="T">Script type attached to the scene.</typeparam>
    /// <returns>Instantiated scene.</returns>
    /// <exception cref="InvalidOperationException">Thrown when type does not have a
    /// <see cref="ScriptPathAttribute" />.</exception>
    public T LoadScene<T>(bool autoFree = true) where T : Node
    {
        // get script path given to a class by the Godot source generators.
        var attr = typeof(T).GetCustomAttribute<ScriptPathAttribute>()
            ?? throw new InvalidOperationException(
                $"Type '{typeof(T)}' does not have a ScriptPathAttribute"
            );
        var path = Path.ChangeExtension(attr.Path, ".tscn");
        return LoadScene<T>(path, autoFree);
    }

    /// <summary>
    /// Loads a scene from the given path and instantiates it but does not add it to the tree.
    /// </summary>
    /// <typeparam name="T">Scene's root node type.</typeparam>
    /// <param name="path">Path to the .tscn scene file.</param>
    /// <param name="autoFree">if set to true, the instance will be automatically freed when the fixture's
    /// <see cref="Cleanup" /> method is called</param>
    /// <exception cref="ArgumentException"></exception>
    public T LoadScene<T>(string path, bool autoFree = true) where T : Node
    {
        var scene = GD.Load<PackedScene>(path);
        if (!Object.IsInstanceValid(scene))
        {
            throw new ArgumentException("Could not load scene from path, it very likely does not exist: " + path);
        }

        var node = scene.Instantiate();

        if (node is not T instance)
        {
            throw new ArgumentException("The root node of the scene is not of the expected type: " + typeof(T).FullName);
        }

        return autoFree ? AutoFree(instance) : instance;
    }

    /// <summary>
    /// Schedules the given object to be automatically freed when this fixture is cleaned up. Returns the object that was
    /// given to it so this call can be nicely chained.
    /// </summary>
    /// <typeparam name="T">Scene's root node type.</typeparam>
    /// <param name="toFree">Godot object to automatically free.</param>
    public T AutoFree<T>(T toFree) where T : Object
    {
        // References are automatically freed, so calling AutoFree on references is wrong.
        if (toFree is RefCounted)
        {
            Log.Error("Trying to auto-free an object that extends RefCounted. I will ignore this but you should probably not do this.");
            return toFree;
        }

        AddCleanupStep(() =>
        {
            // it can happen that a node is already freed when the auto-free is running because
            // the test freed it, or a parent node was freed.
            if (!Object.IsInstanceValid(toFree))
            {
                return;
            }

            if (toFree is Node node)
            {
                // if the node has a valid parent, remove it from the parent first
                if (Object.IsInstanceValid(node.GetParent()))
                {
                    node.GetParent().RemoveChild(node);
                }
                // use QueueFree for nodes to ensure it is safe to free them.
                node.QueueFree();
            }
            else
            {
                // normal objects can be freed directly.
                toFree.Free();
            }
        });
        return toFree;
    }

    /// <summary>
    /// Runs all accumulated cleanup steps. Cleanup steps are run in reverse order in which they were added (e.g.
    /// last added is run first).
    /// </summary>
    public async Task Cleanup()
    {
        // ensure we run on the main thread.
        await Tree.NextFrame();

        if (_cleanupSteps.Count > 0)
        {
            // run cleanup in reverse order, so inner cleanup is done first.
            _cleanupSteps.Reverse();

            Log.Info($"Running {_cleanupSteps.Count} cleanup actions.");
            // run all cleanup actions.
            foreach (var action in _cleanupSteps)
            {
                await action();
            }

            _cleanupSteps.Clear();
        }
    }

    /// <summary>
    /// Allows to register an asynchronous cleanup action that runs when <see cref="Cleanup"/> is called.
    /// </summary>
    /// <param name="action">Action to automatically execute.</param>
    protected void AddCleanupStep(Func<Task> action)
    {
        _cleanupSteps.Add(action);
    }

    /// <summary>
    /// Allows to register a synchronous cleanup action that runs when <see cref="Cleanup"/> is called
    /// </summary>
    /// <param name="action">Action to automatically execute.</param>
    public void AddCleanupStep(Action action)
    {
#pragma warning disable 1998
        _cleanupSteps.Add(async () => action());
#pragma warning restore 1998
    }
}
