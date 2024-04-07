using System.Collections;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class Flasher : MonoBehaviour, IFlasher
    {
        public Color FlashColor;

        [SerializeField]
        private Renderer _renderer;

        private IEnumerator flashCoroutine;

        public void Flash(float duration)
        {
            if (flashCoroutine != null) return;

            flashCoroutine = DoFlash(duration);
            StartCoroutine(flashCoroutine);
        }

        protected virtual IEnumerator DoFlash(float duration)
        {
            float timer = 0;
            var originalColor = _renderer.material.color;
            while (timer < duration)
            {
                _renderer.material.color = Color.Lerp(FlashColor, originalColor, timer / duration);
                timer += Time.deltaTime;
                yield return null;
            }
        }
    }
}
