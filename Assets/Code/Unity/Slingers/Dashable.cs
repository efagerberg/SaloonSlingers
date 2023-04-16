using System.Collections;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class Dashable : MonoBehaviour
    {
        public DashConfig DashConfig { get; private set; }

        [SerializeField]
        private float startingDashSpeed = 6f;
        [SerializeField]
        private uint startingDashes = 3;
        [SerializeField]
        private float startingDashCooldown = 3;
        [SerializeField]
        private float startingDashDuration = 0.25f;
        [SerializeField]
        private float startingPointRecoveryPeriod = 1f;

        private bool canDash = true;

        public void Dash(CharacterController controller, Vector3 forward)
        {
            if (!canDash) return;
            IEnumerator coroutine = DoDash(controller, forward);
            StartCoroutine(coroutine);
        }

        private IEnumerator DoDash(CharacterController controller, Vector3 forward)
        {
            canDash = false;
            DashConfig.DashPoints.Value -= 1;
            float currentDuration = DashConfig.Duration;
            while (currentDuration > 0)
            {
                controller.Move(DashConfig.Speed * Time.deltaTime * forward);
                currentDuration -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForSeconds(DashConfig.CoolDown);
            canDash = DashConfig.DashPoints.Value > 0;

            // Assumes the cooldown is always smaller than the recovery period
            // Game-wise this makes sense, why would the player be allowed to regen
            // points faster than they can use them.
            yield return new WaitForSeconds(DashConfig.PointRecoveryPeriod - DashConfig.CoolDown);
            DashConfig.DashPoints.Value += 1;
            canDash = true;
        }

        private void Start()
        {
            DashConfig = new DashConfig(
                startingDashes,
                startingDashSpeed,
                startingDashDuration,
                startingDashCooldown,
                startingPointRecoveryPeriod
            );
        }
    }
}
