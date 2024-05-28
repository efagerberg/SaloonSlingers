using System.Collections.Generic;

using UnityEngine.Events;

namespace SaloonSlingers.Unity
{
    public enum LevelResult
    {
        UNDEFINED, PLAYER_KILLED, ALL_ENEMIES_KILLED
    }

    public class LevelCompleteNotifier
    {
        public UnityEvent<LevelResult> LevelCompleted = new();
        private readonly IDictionary<string, int> enemiesLeft;

        public LevelCompleteNotifier(IReadOnlyDictionary<string, int> enemyManifest)
        {
            enemiesLeft = new Dictionary<string, int>(enemyManifest);
        }

        public void OnEnemyKilled(Actor.Actor sender)
        {
            enemiesLeft[sender.name]--;
            if (enemiesLeft[sender.name] == 0)
            {
                enemiesLeft.Remove(sender.name);
                if (enemiesLeft.Count == 0)
                    LevelCompleted?.Invoke(LevelResult.ALL_ENEMIES_KILLED);
            }

            sender.OnKilled.RemoveListener(OnEnemyKilled);
        }

        public void OnPlayerKilled(Actor.Actor sender)
        {
            LevelCompleted?.Invoke(LevelResult.PLAYER_KILLED);
        }
    }
}

