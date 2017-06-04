using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class PlayerStats : IPlayerStats
{
    public bool IsDead { get; private set; } = false;

    private float health = 1f;
    private float healthRegen = 0.01f;
    public float Health
    {
        get { return health; }
        private set { health = Mathf.Clamp(value, 0, 1f); }
    }

    private float stamina = 1f;
    private float staminaBurn = 1f;
    private float staminaRegen = 0.1f;
    public float Stamina
    {
        get { return stamina; }
        private set { stamina = Mathf.Clamp(value, 0, 1f); }
    }

    public PlayerStats() { }

    public void Update(Dictionary<string, bool> inputInfo, float deltaTime)
    {
        Stamina += staminaRegen * deltaTime;
        Health += healthRegen * deltaTime;
        if (PauseMenu.IsOn) return;
        if (inputInfo["IsRunning"] && Stamina > 0.01f)
        {
            Stamina -= staminaBurn * deltaTime;
        }
    }

    public void Reset()
    {
        Stamina = 1f;
        Health = 1f;
        IsDead = false;
    }

    public void EmptyStats()
    {
        Stamina = 0f;
        Health = 0f;
    }

    public void TakeDamage(float _amount)
    {
        if (IsDead) return;
        Health -= _amount;
        if (Health == 0)
        {
            IsDead = true;
        }
    }
}
