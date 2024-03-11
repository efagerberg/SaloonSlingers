using System;
using System.Collections;

using SaloonSlingers.Core;

using UnityEngine;
using UnityEngine.Events;

namespace SaloonSlingers.Unity.Actor
{
    public class DeathDetector : MonoBehaviour, IActor
    {
        public IReadOnlyAttribute HitPoints { get; set; }
        public UnityEvent OnKilled;
        public UnityEvent OnReset;
        public event EventHandler Killed;

        [SerializeField]
        private float deathDelaySeconds = 0.5f;

        public void ResetActor()
        {
            OnReset?.Invoke();
        }

        private void OnEnable()
        {
            if (HitPoints == null) return;

            HitPoints.Depleted += OnHealthDepleted;
        }

        private void OnDisable()
        {
            if (HitPoints == null) return;

            HitPoints.Depleted -= OnHealthDepleted;
        }

        private void Start()
        {
            HitPoints ??= GetComponent<Attributes>().Registry[AttributeType.Health];
            HitPoints.Depleted += OnHealthDepleted;
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
