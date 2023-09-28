using System.Collections.Generic;

using Newtonsoft.Json;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class SaloonManager : MonoBehaviour
    {
        public static SaloonManager Instance { get; private set; }

        public Saloon Saloon { get; private set; }
        public ISpawner<GameObject> CardSpawner { get => cardSpawner; }
        public ISpawner<GameObject> HandInteractableSpawner { get => handInteractableSpawner; }
        public EnemySpawner EnemySpawner { get => enemySpawner; }
        public GameObject Player { get => player; } 

        [SerializeField]
        private TextAsset configTextAsset;
        [SerializeField]
        private CardSpawner cardSpawner;
        [SerializeField]
        private HandInteractableSpawner handInteractableSpawner;
        [SerializeField]
        private EnemySpawner enemySpawner;
        [SerializeField]
        private GameObject player;

        public void LoadRules()
        {
            var rawConfig = JsonConvert.DeserializeObject<RawConfig>(configTextAsset.text);
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

        private void Awake()
        {
            LoadRules();
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
    }

    struct RawConfig
    {
        public string SaloonId { get; set; }
        public float InterestRisk { get; set; }
        public IDictionary<string, int> EnemyManifest { get; set; }
        public string HouseGame { get; set; }
    }
}

