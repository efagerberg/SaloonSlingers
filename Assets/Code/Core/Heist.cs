using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SaloonSlingers.Core
{
    [Serializable]
    public struct Heist
    {
        public readonly bool Complete { get => enemyInventory.Manifest.Count == 0; }
        public float InterestRisk { get; private set; }
        public string SaloonId { get; private set; }
        public CardGame[] HouseGames { get; private set; }

        private EnemyInventory enemyInventory;
        private Random random;


        public readonly string GetRandomEnemy()
        {
            var enemiesAvailable = enemyInventory.Manifest;
            if (enemiesAvailable.Count == 0) return null;

            int randomIndex = random.Next(enemiesAvailable.Count);
            return enemyInventory.GetEnemy(enemiesAvailable.Keys.ElementAt(randomIndex));
        }

        public static Heist Load(HeistConfig config)
        {
            var rules = new Heist
            {
                SaloonId = config.SaloonId,
                InterestRisk = config.InterestRisk,
                HouseGames = config.HouseGames.Select(CardGame.Load).ToArray(),
                enemyInventory = new EnemyInventory(config.EnemyInventory),
                random = new Random()
            };
            return rules;
        }
    }

    public struct HeistConfig
    {
        public string SaloonId { get; set; }
        public float InterestRisk { get; set; }
        public IDictionary<string, int> EnemyInventory { get; set; }
        public CardGameConfig[] HouseGames { get; set; }
    }

    class EnemyInventory
    {
        private readonly IDictionary<string, int> manifest;

        public EnemyInventory(IDictionary<string, int> manifest)
        {
            this.manifest = manifest;
        }

        public string GetEnemy(string name)
        {
            if (manifest.ContainsKey(name))
            {
                int nLeft = manifest[name];
                nLeft--;
                if (nLeft == 0) manifest.Remove(name);
                return name;
            }
            return null;
        }

        public IReadOnlyDictionary<string, int> Manifest { get => new ReadOnlyDictionary<string, int>(manifest); }
    }
}