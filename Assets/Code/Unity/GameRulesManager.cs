using System;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public delegate void GameRulesChangedHandler(GameRules sender, EventArgs e);

    public class GameRulesManager : MonoBehaviour
    {
        public GameRules GameRules { get; private set; }
        public event GameRulesChangedHandler OnGameRulesChanged;

        [SerializeField]
        private string ConfigPath;

        public void LoadRules(string configPath)
        {
            ConfigPath = configPath;
            TextAsset rulesTextAsset = Resources.Load<TextAsset>(ConfigPath);
            GameRules = GameRules.Load(rulesTextAsset.text);
            OnGameRulesChanged?.Invoke(GameRules, EventArgs.Empty);
        }

        private void Awake() => LoadRules(ConfigPath);
    }
}

