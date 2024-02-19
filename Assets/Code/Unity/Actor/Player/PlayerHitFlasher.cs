using System.Collections;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class PlayerHitFlasher : MonoBehaviour, IHitFlasher
    {
        [SerializeField]
        private CanvasGroup flashCanvasGroup;

        public IEnumerator Flash(float duration)
        {
            flashCanvasGroup.gameObject.SetActive(true);
            yield return Fader.Fade((alpha) => flashCanvasGroup.alpha = alpha, duration, 1, 0);
            flashCanvasGroup.gameObject.SetActive(false);
        }
    }
}
