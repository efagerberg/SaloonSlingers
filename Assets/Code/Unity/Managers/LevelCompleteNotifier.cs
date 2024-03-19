using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace SaloonSlingers.Unity
{
    public class LevelCompleteNotifier
    {
        public UnityEvent LevelCompleted = new();
        private readonly IDictionary<string, int> enemiesLeft;

        public LevelCompleteNotifier(IReadOnlyDictionary<string, int> enemyManifest)
        {
            enemiesLeft = new Dictionary<string, int>(enemyManifest);
        }

        public void OnEnemyKilled(GameObject sender)
        {
            enemiesLeft[sender.name]--;
            if (enemiesLeft[sender.name] == 0)
            {
                enemiesLeft.Remove(sender.name);
                if (enemiesLeft.Count == 0)
                    LevelCompleted?.Invoke();
            }

            var actor = sender.GetComponent<Actor.Actor>();
            actor.OnKilled.RemoveListener(OnEnemyKilled);
        }
    }
}

