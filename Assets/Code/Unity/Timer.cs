using UnityEngine;
using UnityEngine.Events;

namespace SaloonSlingers.Unity.Actor
{
    public class Timer : MonoBehaviour
    {
        public UnityEvent Expired = new();

        [SerializeField]
        [Range(0f, 1000f)]
        private float duration = 1f;

        private Core.Timer timer;

        private void OnEnable()
        {
            timer = new Core.Timer(duration);
        }

        private void Update()
        {
            if (!timer.Tick(Time.deltaTime)) return;

            Expired.Invoke();
            enabled = false;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            timer.Reset(duration);
        }
#endif

        private void OnDisable()
        {
            timer.Reset(duration);
        }
    }
}
