using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace SaloonSlingers.Unity.Actor
{
    public class PlayerFlasher : MonoBehaviour, IFlasher
    {
        [SerializeField]
        private CanvasGroup flashCanvasGroup;
        [SerializeField]
        private Image flashImage;
        [SerializeField]
        private float duration;

        private CoroutineTask task;

        public void Flash()
        {
            task ??= new(this);
            task.Run(() => DoFlash(duration));
        }

        private IEnumerator DoFlash(float duration)
        {
            flashCanvasGroup.gameObject.SetActive(true);
            yield return Fader.Fade((alpha) => flashCanvasGroup.alpha = alpha, duration, 1, 0);
            flashCanvasGroup.gameObject.SetActive(false);
        }
    }
}
