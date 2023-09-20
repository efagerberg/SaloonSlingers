using System;

using Newtonsoft.Json;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public delegate void CardGameChangedHandler(CardGame sender, EventArgs e);

    public class CardGameManager : MonoBehaviour
    {
        public CardGame Game { get; private set; }
        public event CardGameChangedHandler OnGameRulesChanged;

        [SerializeField]
        private TextAsset ConfigTextAsset;

        public void LoadRules()
        {
            var config = JsonConvert.DeserializeObject<CardGameConfig>(ConfigTextAsset.text);
            Game = CardGame.Load(config);
            OnGameRulesChanged?.Invoke(Game, EventArgs.Empty);
        }

        private void Awake() => LoadRules();
    }
}

