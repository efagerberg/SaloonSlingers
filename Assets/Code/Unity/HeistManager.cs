using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public delegate void CardGameChangedHandler(CardGame sender, EventArgs e);

    public class HeistManager : MonoBehaviour
    {
        public CardGame Game { get; private set; }
        public event CardGameChangedHandler OnGameRulesChanged;

        [SerializeField]
        private TextAsset ConfigTextAsset;

        public void LoadRules()
        {
            var rawConfig = JsonConvert.DeserializeObject<RawHeistConfig>(ConfigTextAsset.text);
            var cardGameTextAssets = rawConfig.HouseGames.Select(x => Resources.Load<TextAsset>($"CardGameConfigs/{x}"))
                                                         .Select(x => x.text);
            var config = new HeistConfig
            {
                SaloonId = rawConfig.SaloonId,
                EnemyInventory = rawConfig.EnemyInventory,
                InterestRisk = rawConfig.InterestRisk,
                HouseGames = cardGameTextAssets.Select(JsonConvert.DeserializeObject<CardGameConfig>).ToArray()
            };
            var heist = Heist.Load(config);
            Game = heist.HouseGames[0];
            OnGameRulesChanged?.Invoke(Game, EventArgs.Empty);
        }

        private void Awake() => LoadRules();
    }

    struct RawHeistConfig
    {
        public string SaloonId { get; set; }
        public float InterestRisk { get; set; }
        public IDictionary<string, int> EnemyInventory { get; set; }
        public string[] HouseGames { get; set; }
    }
}

