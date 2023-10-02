using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    [RequireComponent(typeof(SceneLoader))]
    public class GameManager : Singleton<GameManager>
    {
        public Saloon Saloon { get; private set; }
        public SceneLoader SceneLoader { get; private set; }

        [SerializeField]
        private TextAsset saloonConfigAsset;

        public void LoadSaloon(Saloon saloon)
        {
            Saloon = saloon;
            SceneLoader.LoadScene(Saloon.Id);
        }

        protected override void Awake()
        {
            base.Awake();
            SceneLoader = GetComponent<SceneLoader>();
            if (saloonConfigAsset != null)
                Saloon = LevelManager.Load(saloonConfigAsset.text);
        }
    }
}
