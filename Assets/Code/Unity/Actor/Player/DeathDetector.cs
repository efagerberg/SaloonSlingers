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

        private IReadOnlyAttribute hitPoints;

        public void ResetActor()
        {
            OnReset?.Invoke();
        }

        private void OnEnable()
        {
            if (hitPoints == null) return;

            hitPoints.Depleted += OnHealthDepleted;
        }

        private void OnDisable()
        {
            if (hitPoints == null) return;

            hitPoints.Depleted -= OnHealthDepleted;
        }

        private void Start()
        {
            hitPoints ??= GetComponent<Attributes>().Registry[AttributeType.Health];
            hitPoints.Depleted += OnHealthDepleted;
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
