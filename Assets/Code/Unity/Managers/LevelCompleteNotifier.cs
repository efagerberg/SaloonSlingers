using System;
using System.Collections.Generic;

using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class LevelCompleteNotifier
    {
        public EventHandler LevelComplete;
        private readonly IDictionary<string, int> enemiesLeft;

        public LevelCompleteNotifier(IReadOnlyDictionary<string, int> enemyManifest)
        {
            enemiesLeft = new Dictionary<string, int>(enemyManifest);
        }

        public void OnEnemyKilled(object sender, EventArgs args)
        {
            var go = (GameObject)sender;
            enemiesLeft[go.name]--;
            if (enemiesLeft[go.name] == 0)
            {
                enemiesLeft.Remove(go.name);
                if (enemiesLeft.Count == 0)
                    LevelComplete?.Invoke(this, EventArgs.Empty);
            }

            var actor = go.GetComponent<IActor>();
            actor.Killed -= OnEnemyKilled;
        }
    }
}

