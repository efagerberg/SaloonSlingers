using System;
using UnityEngine;

[Serializable]
public class PlayerController : IPlayerController
{
    private const float RUN_SPEED = 3f;
    private const float WALK_SPEED = 1f;

    private Vector3 m_velocity;
    public Vector3 Velocity { get { return m_velocity; } }

    private IPlayerStats playerStats;

    public PlayerController(IPlayerStats playerStats)
    {
        this.playerStats = playerStats;
    }

    public void HandlePause()
    {
        m_velocity = Vector3.zero;
    }

    public void HandleMovement(Transform _transform, Vector2 input, bool _isRunning)
    {
        Vector3 _movHorizontal = _transform.right * input.x;
        Vector3 _movVerical = _transform.forward * input.y;

        var speed = WALK_SPEED;
        if (_isRunning && playerStats.Stamina > 0.01f)
        {
            speed = RUN_SPEED;
        }
        m_velocity = (_movHorizontal + _movVerical).normalized * speed;
    }
}