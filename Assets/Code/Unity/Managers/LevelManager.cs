using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class LevelManager : Singleton<LevelManager>
    {
        public ISpawner<GameObject> CardSpawner { get => cardSpawner; }
        public ISpawner<GameObject> HandInteractableSpawner { get => handInteractableSpawner; }
        public ISpawner<GameObject> PickupSpawner { get => pickupSpawner; }
        public ISpawner<GameObject> PickupElevatorSpawner { get => pickupElevatorSpawner; }
        public EnemyManager EnemyManager;
        public GameObject Player { get => player; }

        [SerializeField]
        private ActorSpawner cardSpawner;
        [SerializeField]
        private ActorSpawner handInteractableSpawner;
        [SerializeField]
        private ActorSpawner pickupSpawner;
        [SerializeField]
        private ActorSpawner pickupElevatorSpawner;
        [SerializeField]
        private GameObject player;

        private LevelCompleteNotifier levelCompleteNotifier;

        public static Saloon Load(string configFileContents)
        {
            var rawConfig = JsonConvert.DeserializeObject<RawConfig>(configFileContents);
            var cardGameTextAsset = Resources.Load<TextAsset>($"CardGameConfigs/{rawConfig.HouseGame}").text;
            var config = new SaloonConfig
            {
                Id = rawConfig.SaloonId,
                Name = rawConfig.Name,
                Description = rawConfig.Description,
                EnemyManifest = rawConfig.EnemyManifest,
                InterestRisk = rawConfig.InterestRisk,
                HouseGame = JsonConvert.DeserializeObject<CardGameConfig>(cardGameTextAsset)
            };
            return Saloon.Load(config);
        }

        public static IEnumerable<Saloon> GetSaloons()
        {
            foreach (var configTextAsset in Resources.LoadAll<TextAsset>($"SaloonConifgs"))
            {
                yield return Load(configTextAsset.text);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            levelCompleteNotifier = new(GameManager.Instance.Saloon.EnemyInventory.Manifest);
        }

        private void OnEnable()
        {
            levelCompleteNotifier.LevelComplete += OnLevelComplete;
            EnemyManager.EnemyKilled += levelCompleteNotifier.OnEnemyKilled;
        }

        private void OnDisable()
        {
            levelCompleteNotifier.LevelComplete -= OnLevelComplete;
            EnemyManager.EnemyKilled -= levelCompleteNotifier.OnEnemyKilled;
        }

        private void OnLevelComplete(object sender, EventArgs args)
        {
            GameManager.Instance.SceneLoader.LoadScene("StartingScene");
        }
    }

    struct RawConfig
    {
        public string SaloonId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float InterestRisk { get; set; }
        public IDictionary<string, int> EnemyManifest { get; set; }
        public string HouseGame { get; set; }
    }
}

