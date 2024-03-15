using System;
using System.Collections;

using SaloonSlingers.Core;

using UnityEngine;
using UnityEngine.Events;

namespace SaloonSlingers.Unity.Actor
{
    public class DeathDetector : MonoBehaviour, IActor
    {
        public UnityEvent OnKilled = new();
        public UnityEvent OnReset = new();
        public event EventHandler Killed;

        [SerializeField]
        private float deathDelaySeconds = 0f;

        private Attributes attributes;

        public void ResetActor()
        {
            foreach (var attribute in attributes.Registry.Values)
                attribute.Reset();
            OnReset?.Invoke();
        }

        private void OnEnable()
        {
            if (attributes == null) return;

            attributes.Registry[AttributeType.Health].Depleted += OnHealthDepleted;
        }

        private void OnDisable()
        {
            if (attributes == null) return;

            attributes.Registry[AttributeType.Health].Depleted -= OnHealthDepleted;
        }

        private void Start()
        {
            attributes ??= GetComponent<Attributes>();
            attributes.Registry[AttributeType.Health].Depleted += OnHealthDepleted;
        }

        private void OnHealthDepleted(IReadOnlyAttribute sender, EventArgs e)
        {
            OnKilled?.Invoke();
            StartCoroutine(nameof(DelayDeath));
        }

        private IEnumerator DelayDeath()
        {
            yield return new WaitForSeconds(deathDelaySeconds);
            Killed?.Invoke(gameObject, EventArgs.Empty);
        }
    }
}
