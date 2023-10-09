using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SaloonSlingers.Unity
{
    public class SceneLoader : MonoBehaviour
    {
        public Image TransitionImage;

        [SerializeField]
        private float transitionDuration = 1f;

        private AsyncOperation loadOperation;

        public void LoadScene(string sceneName)
        {
            if (SceneManager.GetActiveScene().name == sceneName || loadOperation != null) return;
            StartCoroutine(PerformSceneTransition(sceneName));
        }

        public IEnumerator PerformSceneTransition(string sceneName)
        {
            if (TransitionImage != null)
            {
                TransitionImage.gameObject.SetActive(true);
                yield return Fader.FadeTo(TransitionImage, 1, transitionDuration / 2f);
            }
            loadOperation = SceneManager.LoadSceneAsync(sceneName);
            loadOperation.allowSceneActivation = false;

            while (!loadOperation.isDone)
            {
                if (loadOperation.progress >= 0.9f)
                    loadOperation.allowSceneActivation = true;
                yield return null;
            }

            if (TransitionImage != null)
            {
                Color c = TransitionImage.color;
                c.a = 1;
                TransitionImage.color = c;
                TransitionImage.gameObject.SetActive(true);
                yield return Fader.FadeTo(TransitionImage, 0, transitionDuration / 2f);
                TransitionImage.gameObject.SetActive(false);
                var originalColor = TransitionImage.color;
                originalColor.a = 0;
                TransitionImage.color = originalColor;
            }
            loadOperation = null;
        }
    }
}
