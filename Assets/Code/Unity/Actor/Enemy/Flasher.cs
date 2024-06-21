using System.Collections;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{

    public class Flasher : MonoBehaviour, IFlasher
    {
        public Color FlashColor;

        [SerializeField]
        private Renderer _renderer;
        [SerializeField]
        private float duration;

        private CoroutineTask task;
        private Core.Timer timer;

        public void Flash()
        {
            task.Run(() => DoFlash(duration));
        }

        private void Awake()
        {
            timer = new();
            task = new(this);
        }

        private IEnumerator DoFlash(float duration)
        {
            var originalColor = _renderer.material.color;
            timer.Start(duration);
            while (!timer.Tick(Time.deltaTime))
            {
                _renderer.material.color = Color.Lerp(FlashColor, originalColor, timer.Elapsed / timer.Duration);
                yield return null;
            }
        }
    }
}
