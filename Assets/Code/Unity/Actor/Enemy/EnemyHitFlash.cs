using System.Collections;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class EnemyHitFlash : MonoBehaviour, IHitFlasher
    {
        [SerializeField]
        private Renderer _renderer;
        [SerializeField]
        private Color hitColor;

        public IEnumerator Flash(float duration)
        {
            float timer = 0;
            var originalColor = _renderer.material.color;
            while (timer < duration)
            {
                _renderer.material.color = Color.Lerp(hitColor, originalColor, timer / duration);
                timer += Time.deltaTime;
                yield return null;
            }
        }
    }
}
