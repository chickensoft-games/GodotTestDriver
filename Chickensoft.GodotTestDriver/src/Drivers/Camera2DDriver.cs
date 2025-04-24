namespace Chickensoft.GodotTestDriver.Drivers;

using System;
using System.Threading.Tasks;
using Godot;
using GodotTestDriver.Util;

/// <summary>
/// Driver for the Camera2D node.
/// </summary>
/// <typeparam name="T">Camera2D type.</typeparam>
public class Camera2DDriver<T> : Node2DDriver<T> where T : Camera2D
{
    /// <summary>
    /// Creates a new generic Camera2DDriver.
    /// </summary>
    /// <param name="producer">Producer that creates a Camera2D subclass.</param>
    /// <param name="description">Driver description.</param>
    public Camera2DDriver(Func<T> producer, string description = "") : base(producer, description)
    {
    }

    /// <summary>
    /// Starts moving the given position into the view of the camera. This does NOT wait for the camera to
    /// finish moving.
    /// </summary>
    /// <param name="worldPosition">World position to move into view.</param>
    /// <seealso cref="MoveIntoView(Vector2, float)"/>
    /// <seealso cref="WaitUntilSteady(float)"/>
    public void StartMoveIntoView(Vector2 worldPosition)
    {
        PresentRoot.GlobalPosition = worldPosition;
    }

    /// <summary>
    /// Moves the given position into the view of the camera. This will wait for the given amount of seconds
    /// until the camera no longer moves.
    /// </summary>
    /// <param name="worldPosition">World position.</param>
    /// <param name="timeoutSeconds">Timeout, in seconds.</param>
    public async Task<bool> MoveIntoView(Vector2 worldPosition, float timeoutSeconds)
    {
        StartMoveIntoView(worldPosition);
        return await WaitUntilSteady(timeoutSeconds);
    }

    /// <summary>
    /// Waits until the camera steady (e.g. not moving for three frames).
    /// </summary>
    /// <param name="timeoutSeconds">Timeout, in seconds.</param>
    public async Task<bool> WaitUntilSteady(float timeoutSeconds)
    {
        var timeout = new Timeout(timeoutSeconds);
        var screenPos = PresentRoot.GetScreenCenterPosition();
        // we treat the camera as steady when it hasn't moved over 3 frames.
        var frameCount = 0;

        do
        {
            var newScreenPos = PresentRoot.GetScreenCenterPosition();
            if ((newScreenPos - screenPos).LengthSquared() < 0.001)
            {
                frameCount++;
            }
            else
            {
                frameCount = 0;
            }

            if (frameCount >= 3)
            {
                return true;
            }

            screenPos = newScreenPos;
            await PresentRoot.GetTree().NextFrame();
        } while (!timeout.IsReached);

        return false;
    }
}

/// <summary>
/// Driver for the Camera2D node.
/// </summary>
public sealed class Camera2DDriver : Camera2DDriver<Camera2D>
{
    /// <summary>
    /// Creates a new Camera2DDriver.
    /// </summary>
    /// <param name="producer">Producer that creates a Camera2D subclass.</param>
    /// <param name="description">Driver description.</param>
    public Camera2DDriver(Func<Camera2D> producer, string description = "") : base(producer, description)
    {
    }
}
