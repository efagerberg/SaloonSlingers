using System.Collections.Generic;

using Newtonsoft.Json;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class HeistManager : MonoBehaviour
    {
        public Heist Heist { get; private set; }

        [SerializeField]
        private TextAsset ConfigTextAsset;

        public void LoadRules()
        {
            var rawConfig = JsonConvert.DeserializeObject<RawHeistConfig>(ConfigTextAsset.text);
            var cardGameTextAsset = Resources.Load<TextAsset>($"CardGameConfigs/{rawConfig.HouseGame}").text;
            var config = new HeistConfig
            {
                SaloonId = rawConfig.SaloonId,
                EnemyManifest = rawConfig.EnemyManifest,
                InterestRisk = rawConfig.InterestRisk,
                HouseGame = JsonConvert.DeserializeObject<CardGameConfig>(cardGameTextAsset)
            };
            Heist = Heist.Load(config);
        }

        private void Awake() => LoadRules();
    }

    struct RawHeistConfig
    {
        public string SaloonId { get; set; }
        public float InterestRisk { get; set; }
        public IReadOnlyDictionary<string, int> EnemyManifest { get; set; }
        public string HouseGame { get; set; }
    }
}

