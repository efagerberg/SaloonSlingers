using System.Collections.Generic;

public interface IPlayerStats
{
    float Health { get; }
    bool IsDead { get; }
    float Stamina { get; }

    void Reset();
    void TakeDamage(float _amount);
    void Update(Dictionary<string, bool> inputInfo, float deltaTime);
    void EmptyStats();
}