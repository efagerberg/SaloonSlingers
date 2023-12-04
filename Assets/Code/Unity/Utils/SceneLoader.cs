using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace SaloonSlingers.Unity
{
    public class SceneLoader : MonoBehaviour
    {
        public CanvasGroup TransitionCanvasGroup;

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
            if (TransitionCanvasGroup != null)
            {
                TransitionCanvasGroup.alpha = 0;
                TransitionCanvasGroup.gameObject.SetActive(true);
                yield return Fader.Fade((alpha) => TransitionCanvasGroup.alpha = alpha, transitionDuration / 2f, endAlpha: 1);
            }
            loadOperation = SceneManager.LoadSceneAsync(sceneName);
            loadOperation.allowSceneActivation = false;

            while (!loadOperation.isDone)
            {
                if (loadOperation.progress >= 0.9f)
                    loadOperation.allowSceneActivation = true;
                yield return null;
            }

            if (TransitionCanvasGroup != null)
            {
                TransitionCanvasGroup.alpha = 1;
                TransitionCanvasGroup.gameObject.SetActive(true);
                yield return Fader.Fade(
                    (alpha) => TransitionCanvasGroup.alpha = alpha,
                    transitionDuration / 2f,
                    startAlpha: TransitionCanvasGroup.alpha, endAlpha: 1
                );
                TransitionCanvasGroup.gameObject.SetActive(false);
                TransitionCanvasGroup.alpha = 0;
            }
            loadOperation = null;
        }
    }
}
