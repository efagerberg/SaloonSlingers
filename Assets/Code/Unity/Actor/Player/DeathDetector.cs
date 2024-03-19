using System;
using System.Collections;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class DeathDetector : Actor
    {
        [SerializeField]
        private float deathDelaySeconds = 0f;

        private Attributes attributes;

        public override void ResetActor()
        {
            foreach (var attribute in attributes.Registry.Values)
                attribute.Reset();
            OnReset?.Invoke(gameObject);
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
            StartCoroutine(nameof(DelayDeath));
        }

        private IEnumerator DelayDeath()
        {
            yield return new WaitForSeconds(deathDelaySeconds);
            OnKilled?.Invoke(gameObject);
        }
    }
}
