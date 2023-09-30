using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    [RequireComponent(typeof(SceneLoader))]
    public class GameManager : Singleton<GameManager>
    {
        public Saloon Saloon
        {
            get;
            private set;
        }

        private SceneLoader sceneLoader;

        public void LoadSaloon(Saloon saloon)
        {
            Saloon = saloon;
            sceneLoader.LoadScene(Saloon.Id);
        }

        protected override void Awake()
        {
            base.Awake();
            sceneLoader = GetComponent<SceneLoader>();
        }
    }
}