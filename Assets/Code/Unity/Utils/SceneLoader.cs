using UnityEditor;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace SaloonSlingers.Unity
{
    public class SceneLoader : MonoBehaviour
    {
        public void LoadScene(string sceneName, bool forceReload = false)
        {
            if (SceneManager.GetActiveScene().name != sceneName || forceReload)
                SceneManager.LoadScene(sceneName);
        }
    }
}
