using System;
using UnityEngine;

[Serializable]
public class PlayerController : IPlayerController
{
    private const float RUN_SPEED = 3f;
    private const float WALK_SPEED = 1f;

    public Vector3 Velocity { get; private set; }

    private IPlayerStats playerStats;

    public PlayerController(IPlayerStats playerStats)
    {
        this.playerStats = playerStats;
    }

    public void HandlePause()
    {
        Velocity = Vector3.zero;
    }

    public void HandleMovement(Transform _transform, Vector2 _input, bool _isRunning)
    {
        Vector3 _movHorizontal = _transform.right * _input.x;
        Vector3 _movVerical = _transform.forward * _input.y;

        var speed = WALK_SPEED;
        if (_isRunning && playerStats.Stamina > 0.01f)
        {
            speed = RUN_SPEED;
        }

        Vector3 _calculatedVeloity = (_movHorizontal + _movVerical).normalized * speed;
        Velocity = _calculatedVeloity;
        
    }
}