namespace Chickensoft.GodotTestDriver.Tests;
using System.Reflection;
using Chickensoft.GoDotTest;
using Godot;

public partial class Tests : Node2D
{
    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        CallDeferred("Go");
    }

    private void Go()
    {
        GoTest.RunTests(Assembly.GetExecutingAssembly(), this);
    }
}
