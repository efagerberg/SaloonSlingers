using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace SaloonSlingers.Unity
{
    public class SceneLoader : MonoBehaviour
    {
        public CanvasGroup TransitionCanvasGroup;

        [SerializeField]
        private float transitionDuration = 2f;

        private AsyncOperation loadOperation;

        public void LoadScene(string sceneName)
        {
            if (SceneManager.GetActiveScene().name == sceneName || loadOperation != null) return;
            StartCoroutine(PerformSceneTransition(sceneName));
        }

        public IEnumerator PerformSceneTransition(string sceneName)
        {
            yield return Fade(0, 1);

            loadOperation = SceneManager.LoadSceneAsync(sceneName);
            loadOperation.allowSceneActivation = false;

            while (!loadOperation.isDone)
            {
                if (loadOperation.progress >= 0.9f)
                    loadOperation.allowSceneActivation = true;
                yield return null;
            }

            yield return Fade(1, 0);
            loadOperation = null;
        }

        private IEnumerator Fade(float startAlpha, float endAlpha)
        {
            if (TransitionCanvasGroup != null)
            {
                TransitionCanvasGroup.alpha = startAlpha;
                TransitionCanvasGroup.gameObject.SetActive(true);
                yield return Fader.Fade(
                    (alpha) => TransitionCanvasGroup.alpha = alpha,
                    transitionDuration / 2f,
                    startAlpha: TransitionCanvasGroup.alpha, endAlpha: endAlpha
                );
            }
        }
    }
}
