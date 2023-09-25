using System;
using System.Collections.Generic;
using System.Linq;

namespace SaloonSlingers.Core
{
    public struct Heist
    {
        public float InterestRisk { get; private set; }
        public string SaloonId { get; private set; }
        public CardGame HouseGame { get; private set; }
        public EnemyInventory EnemyInventory { get; private set; }

        public static Heist Load(HeistConfig config)
        {
            return new Heist
            {
                SaloonId = config.SaloonId,
                InterestRisk = config.InterestRisk,
                HouseGame = CardGame.Load(config.HouseGame),
                EnemyInventory = new EnemyInventory(config.EnemyManifest),
            };
        }
    }

    public struct HeistConfig
    {
        public string SaloonId { get; set; }
        public float InterestRisk { get; set; }
        public IReadOnlyDictionary<string, int> EnemyManifest { get; set; }
        public CardGameConfig HouseGame { get; set; }
    }

    public readonly struct EnemyInventory
    {
        public readonly IReadOnlyDictionary<string, int> Manifest;
        public bool Completed
        {
            get => remaining.Count == 0;
        }

        private readonly IDictionary<string, int> spawned;
        private readonly IDictionary<string, int> remaining;
        private readonly Random random;

        public EnemyInventory(IReadOnlyDictionary<string, int> manifest)
        {
            Manifest = manifest;
            spawned = new Dictionary<string, int>();
            remaining = new Dictionary<string, int>(manifest);
            random = new Random();
        }

        public void Reset()
        {
            spawned.Clear();
            remaining.Clear();
            foreach (var pair in Manifest)
                remaining.Add(pair.Key, pair.Value);
        }

        public readonly string GetEnemyToSpawn(string name)
        {
            if (!Manifest.ContainsKey(name)) return null;

            spawned.TryGetValue(name, out var spawnedCount);
            if (spawnedCount == Manifest[name]) return null;

            spawned[name] = spawnedCount + 1;
            return name;
        }

        public readonly string GetRandomEnemy()
        {
            if (Completed) return null;

            int randomIndex = random.Next(remaining.Count);
            return GetEnemyToSpawn(remaining.Keys.ElementAt(randomIndex));
        }

        public void RecordDeath(string name)
        {
            if (!spawned.ContainsKey(name)) return;

            remaining[name]--;
            if (remaining[name] == 0) remaining.Remove(name);
        }
    }
}
