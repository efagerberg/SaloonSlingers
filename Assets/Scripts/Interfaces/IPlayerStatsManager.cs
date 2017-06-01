using UnityEngine;

public delegate void OnDeathEvent();
public delegate void OnRespawnEvent();

public interface IPlayerStatsManager
{
    IPlayerStats PlayerStats { get; }
    void TakeDamage(float _amount);
    // Event Handler
    event OnDeathEvent OnDeath;
    event OnRespawnEvent OnRespawn;
}
