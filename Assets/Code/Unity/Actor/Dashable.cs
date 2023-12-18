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

        public void Dash(CharacterController controller, Vector3 forward)
        {
            IEnumerator onStart()
            {
                var originalMask = gameObject.layer;
                gameObject.layer = LayerMask.NameToLayer("Invincible");
                float currentDuration = MetaData.Duration;
                while (currentDuration > 0)
                {
                    controller.Move(Speed * Time.deltaTime * forward);
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
            Points = new Points(startingDashes);
            MetaData = new(startingDashDuration,
                startingDashCooldown,
                startingPointRecoveryPeriod);

        }
    }
}
