using System.Collections;
using System.Collections.Generic;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    [RequireComponent(typeof(SceneLoader))]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public Saloon Saloon {
            get;
            private set;
        }

        private SceneLoader sceneLoader;

        public void LoadSaloon(Saloon saloon)
        {
            Saloon = saloon;
            sceneLoader.LoadScene(Saloon.Id);
        }

        private void Awake()
        {
            sceneLoader = GetComponent<SceneLoader>();
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else Destroy(gameObject);
        }
    }
}
