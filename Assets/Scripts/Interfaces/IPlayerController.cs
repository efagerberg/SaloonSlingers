using UnityEngine;

public interface IPlayerController
{
    Vector3 Velocity { get; }
    void HandleMovement(Transform _transform, Vector2 input, bool _isRunning);
    void HandlePause();
}