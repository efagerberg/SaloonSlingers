using System;
using System.Collections.Generic;
using System.Linq;

namespace SaloonSlingers.Core
{
    public class EnemyInventory
    {
        public readonly IReadOnlyDictionary<string, int> Manifest;
        public bool Empty
        {
            get => remaining.Count == 0;
        }
        public EventHandler Emptied;

        private readonly IDictionary<string, int> remaining;
        private readonly Random random;

        public EnemyInventory(IReadOnlyDictionary<string, int> manifest)
        {
            Manifest = manifest;
            remaining = new Dictionary<string, int>(manifest);
            random = new Random();
        }

        public string GetRandomEnemy()
        {
            if (Empty) return null;

            int randomIndex = random.Next(remaining.Count);
            var name = remaining.Keys.ElementAt(randomIndex);
            remaining[name]--;
            if (remaining[name] == 0)
            {
                remaining.Remove(name);
                Emptied?.Invoke(this, EventArgs.Empty);
            }
            return name;
        }
    }
}
