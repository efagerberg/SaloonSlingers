using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour, IPlayerStatsManager
{
    public event OnDeathEvent OnDeath;
    public event OnRespawnEvent OnRespawn;

    private IPlayerStats m_playerStats = new PlayerStats();
    public IPlayerStats PlayerStats { get { return m_playerStats; } }

    private GameObject[] spawnPoints;

    private void Start()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
    }

    private void Update()
    {
        var inputInfo = new Dictionary<string, bool>();
        inputInfo["IsJumping"] = Input.GetButton("Jump");
        inputInfo["IsRunning"] = Input.GetKey(KeyCode.LeftShift);
        m_playerStats.Update(inputInfo, Time.deltaTime);
    }

    public void Die()
    {
        OnDeath();
        //Spawn DeathFX
        //var _gfx = Instantiate(deathEffect, transform.position, Quaternion.identity);
        //Destroy(_gfx, 3f);
        StartCoroutine(Respawn());
    }

    public IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);
        var spawnPointIndex = Random.Range(0, spawnPoints.Length);
        Transform _spawnPointTransform = spawnPoints[spawnPointIndex].transform;
        transform.position = _spawnPointTransform.position;
        transform.rotation = _spawnPointTransform.rotation;
        m_playerStats.Reset();
        // Buffer for particle spawning
        yield return new WaitForSeconds(0.1f);

        OnRespawn();
        Debug.Log(transform.name + " respawned.");
    }

    public void TakeDamage(float _amount)
    {
        m_playerStats.TakeDamage(_amount);
        Debug.Log(transform.name + " now has " + m_playerStats.Health + " health.");
        if (m_playerStats.IsDead)
        {
            Die();
        }
    }
}

