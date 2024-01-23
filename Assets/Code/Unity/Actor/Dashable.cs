using System;
using System.Collections;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class Dashable : ActionPerformer
    {
        public float Speed { get; set; } = 6f;

        [SerializeField]
        private uint startingDashes = 3;
        [SerializeField]
        private float startingDashCooldown = 3;
        [SerializeField]
        private float startingDashDuration = 0.25f;
        [SerializeField]
        private float startingPointRecoveryPeriod = 1f;

        public void Dash(Action<Vector3> moveFunc, Vector3 forward)
        {
            IEnumerator onStart()
            {
                var originalMask = gameObject.layer;
                gameObject.layer = LayerMask.NameToLayer("Invincible");
                float currentDuration = MetaData.Duration;
                while (currentDuration > 0)
                {
                    moveFunc(Speed * Time.deltaTime * forward);
                    currentDuration -= Time.deltaTime;
                    yield return null;
                }
                gameObject.layer = originalMask;
            }
            IEnumerator coroutine = GetActionCoroutine(onStart);
            if (coroutine == null) return;

            StartCoroutine(coroutine);
        }

        private void Start()
        {
            if (IsInitialized) return;

            Core.Attribute points = new(startingDashes);
            ActionMetaData metaData = new()
            {
                Duration = startingDashDuration,
                Cooldown = startingDashCooldown,
                RecoveryPeriod = startingPointRecoveryPeriod
            };
            Initialize(points, metaData);
        }
    }
}
