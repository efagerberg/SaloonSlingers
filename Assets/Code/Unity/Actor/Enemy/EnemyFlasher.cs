using System.Collections;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{

    public class EnemyFlasher : MonoBehaviour, IFlasher
    {
        public Color FlashColor;

        [SerializeField]
        private Renderer _renderer;
        [SerializeField]
        private float duration;

        private CoroutineTask task;

        public void Flash()
        {
            task ??= new(this, () => DoFlash(duration));
            task.Run();
        }

        private IEnumerator DoFlash(float duration)
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
