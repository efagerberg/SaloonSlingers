using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SaloonSlingers.Core
{
    public delegate void CurrentGameChangedHandler(CardGame sender, EventArgs e);

    [Serializable]
    public struct Heist
    {
        public event CurrentGameChangedHandler OnCurrentGameChanged;
        public readonly bool Complete { get => enemyInventory.Manifest.Count == 0; }
        public float InterestRisk { get; private set; }
        public string SaloonId { get; private set; }
        public CardGame HouseGame { get; private set; }

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
            return new Heist
            {
                SaloonId = config.SaloonId,
                InterestRisk = config.InterestRisk,
                HouseGame = CardGame.Load(config.HouseGame),
                enemyInventory = new EnemyInventory(config.EnemyInventory),
                random = new Random(),
            };
        }
    }

    public struct HeistConfig
    {
        public string SaloonId { get; set; }
        public float InterestRisk { get; set; }
        public IDictionary<string, int> EnemyInventory { get; set; }
        public CardGameConfig HouseGame { get; set; }
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