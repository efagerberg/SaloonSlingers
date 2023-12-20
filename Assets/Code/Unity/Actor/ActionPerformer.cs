using System;
using System.Collections;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    /// <summary>
    /// Abstracts some action to be taken that costs some amount of action points to perform.
    /// </summary>
    public class ActionPerformer : MonoBehaviour
    {
        private bool canPerformAction = true;
        protected bool IsPerforming { get; set; }
        protected Points Points { get; set; }
        protected ActionMetaData MetaData { get; set; }

        private WaitForSeconds cooldownWait;
        private WaitForSeconds recoveryMinusCooldownWait;

        public void Initialize(Points points, ActionMetaData metaData)
        {
            Points = points;
            MetaData = metaData;
            canPerformAction &= Points > 0;
            cooldownWait = new WaitForSeconds(metaData.Cooldown);
            // Assumes the cooldown is always smaller than the recovery period
            // Game-wise this makes sense, why would the player be allowed to regen
            // points faster than they can use them?
            recoveryMinusCooldownWait = new WaitForSeconds(metaData.RecoveryPeriod - metaData.Cooldown);
        }

        public IEnumerator GetActionCoroutine(Func<IEnumerator> action)
        {
            if (!canPerformAction) return null;
            return DoAction(action);
        }

        private IEnumerator DoAction(Func<IEnumerator> action)
        {
            Debug.Log("Prod");
            Debug.Log(MetaData.Duration + MetaData.RecoveryPeriod);
            canPerformAction = false;
            Points.Decrement();
            IsPerforming = true;
            yield return StartCoroutine(action());
            IsPerforming = false;

            yield return cooldownWait;
            canPerformAction = Points > 0;

            yield return recoveryMinusCooldownWait;
            Points.Increment();
            canPerformAction = Points > 0;
        }
    }
}
