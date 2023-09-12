using System.Collections;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class Dashable : ActionPerformer
    {
        public ActionPoints Points { get; private set; }

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

        public void Dash(CharacterController controller, Vector3 forward)
        {
            IEnumerator onStart()
            {
                float currentDuration = Points.Duration;
                while (currentDuration > 0)
                {
                    controller.Move(dashSpeed * Time.deltaTime * forward);
                    currentDuration -= Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
            }
            IEnumerator coroutine = GetActionCoroutine(Points, onStart);
            if (coroutine == null) return;

            StartCoroutine(coroutine);
        }

        private void Start()
        {
            Points = new ActionPoints(
                startingDashes,
                startingDashDuration,
                startingDashCooldown,
                startingPointRecoveryPeriod
            );
        }
    }
}
