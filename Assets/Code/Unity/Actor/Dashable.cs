using System.Collections;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class Dashable : ActionPerformer
    {
        public Points Points { get; private set; }

        [SerializeField]
        private float dashSpeed = 6f;
        [SerializeField]
        private uint startingDashes = 3;
        [SerializeField]
        private float startingDashCooldown = 3;
        [SerializeField]
        private float startingDashDuration = 0.25f;
        [SerializeField]
        private float startingPointRecoveryPeriod = 1f;

        private ActionMetaData metaData;

        public void Dash(CharacterController controller, Vector3 forward)
        {
            IEnumerator onStart()
            {
                var originalMask = gameObject.layer;
                gameObject.layer = LayerMask.NameToLayer("Invincible");
                float currentDuration = metaData.Duration;
                while (currentDuration > 0)
                {
                    controller.Move(dashSpeed * Time.deltaTime * forward);
                    currentDuration -= Time.deltaTime;
                    yield return null;
                }
                gameObject.layer = originalMask;
            }
            IEnumerator coroutine = GetActionCoroutine(Points, metaData, onStart);
            if (coroutine == null) return;

            StartCoroutine(coroutine);
        }

        private void Start()
        {
            Points = new Points(startingDashes);
            metaData = new(startingDashDuration,
                startingDashCooldown,
                startingPointRecoveryPeriod);

        }
    }
}
