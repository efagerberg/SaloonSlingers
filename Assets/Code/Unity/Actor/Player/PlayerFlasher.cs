using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace SaloonSlingers.Unity.Actor
{
    public class PlayerFlasher : Flasher
    {
        [SerializeField]
        private CanvasGroup flashCanvasGroup;
        [SerializeField]
        private Image flashImage;

        protected override IEnumerator DoFlash(float duration)
        {
            flashCanvasGroup.gameObject.SetActive(true);
            yield return Fader.Fade((alpha) => flashCanvasGroup.alpha = alpha, duration, 1, 0);
            flashCanvasGroup.gameObject.SetActive(false);
        }
    }
}
