using SaloonSlingers.Core;

using UnityEngine;
using UnityEngine.Events;

namespace SaloonSlingers.Unity.Actor
{
    public class Lifespan : MonoBehaviour
    {
        public UnityEvent OnLifespanEnded = new();

        [SerializeField]
        [Range(0f, 1000f)]
        private float lifespanInSeconds = 1f;

        private Timer timer;

        private void OnEnable()
        {
            timer = new Timer(lifespanInSeconds);
        }

        private void Update()
        {
            if (timer.CheckPassed(Time.deltaTime)) return;

            OnLifespanEnded.Invoke();
            enabled = false;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            timer.Reset(lifespanInSeconds);
        }
#endif

        private void OnDisable()
        {
            timer.Reset(lifespanInSeconds);
        }
    }
}
