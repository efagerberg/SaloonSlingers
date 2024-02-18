using System.Collections.Generic;

namespace SaloonSlingers.Core
{
    public struct Saloon
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public float InterestRisk { get; private set; }
        public CardGame HouseGame { get; private set; }
        public EnemyInventory EnemyInventory { get; private set; }

        public static Saloon Load(SaloonConfig config)
        {
            return new Saloon
            {
                Id = config.Id,
                Name = config.Name,
                Description = config.Description,
                InterestRisk = config.InterestRisk,
                HouseGame = CardGame.Load(config.HouseGame),
                EnemyInventory = new EnemyInventory((IReadOnlyDictionary<string, int>)config.EnemyManifest),
            };
        }
    }

    public struct SaloonConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float InterestRisk { get; set; }
        public IDictionary<string, int> EnemyManifest { get; set; }
        public CardGameConfig HouseGame { get; set; }
    }
}
