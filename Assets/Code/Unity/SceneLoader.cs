using UnityEngine;
using UnityEngine.SceneManagement;

namespace SaloonSlingers.Unity
{
    public class SceneLoader : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
