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

        private float originalLifespanInSeconds;

        private void Awake()
        {
            originalLifespanInSeconds = lifespanInSeconds;
        }

        private void Update()
        {
            lifespanInSeconds = Mathf.Max(0, lifespanInSeconds - Time.deltaTime);
            if (lifespanInSeconds > 0) return;

            OnLifespanEnded.Invoke();
            enabled = false;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            originalLifespanInSeconds = lifespanInSeconds;
        }
#endif

        private void OnDisable()
        {
            lifespanInSeconds = originalLifespanInSeconds;
        }
    }
}
