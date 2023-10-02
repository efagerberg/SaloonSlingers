using UnityEngine;

namespace SaloonSlingers.Unity
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        [SerializeField]
        private bool persist = false;

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                if (persist) DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);
        }
    }
}
