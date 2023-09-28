using System.Collections.Generic;

using Newtonsoft.Json;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class SaloonManager : MonoBehaviour
    {
        public Saloon Saloon { get; private set; }

        [SerializeField]
        private TextAsset ConfigTextAsset;

        public void LoadRules()
        {
            var rawConfig = JsonConvert.DeserializeObject<RawConfig>(ConfigTextAsset.text);
            var cardGameTextAsset = Resources.Load<TextAsset>($"CardGameConfigs/{rawConfig.HouseGame}").text;
            var config = new SaloonConfig
            {
                Id = rawConfig.SaloonId,
                EnemyManifest = rawConfig.EnemyManifest,
                InterestRisk = rawConfig.InterestRisk,
                HouseGame = JsonConvert.DeserializeObject<CardGameConfig>(cardGameTextAsset)
            };
            Saloon = Saloon.Load(config);
        }

        private void Awake() => LoadRules();
    }

    struct RawConfig
    {
        public string SaloonId { get; set; }
        public float InterestRisk { get; set; }
        public IDictionary<string, int> EnemyManifest { get; set; }
        public string HouseGame { get; set; }
    }
}

