using UnityEditor;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace SaloonSlingers.Unity
{
    public class SceneLoader : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            if (SceneManager.GetActiveScene().name != sceneName)
                SceneManager.LoadScene(sceneName);
        }
    }
}
