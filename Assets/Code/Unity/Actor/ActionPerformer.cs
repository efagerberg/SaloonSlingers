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
        public ActionMetaData MetaData => metaData;
        public IReadOnlyPoints Points => points;

        protected bool IsPerforming { get; set; }
        protected bool IsInitialized { get; private set; } = false;
        protected Points points;
        protected ActionMetaData metaData;

        private bool canPerformAction = true;
        private WaitForSeconds cooldownWait;
        private WaitForSeconds recoveryMinusCooldownWait;

        public void Initialize(Points points, ActionMetaData metaData)
        {
            this.points = points;
            this.metaData = metaData;
            canPerformAction &= points > 0;
            cooldownWait = new WaitForSeconds(metaData.Cooldown);
            // Assumes the cooldown is always smaller than the recovery period
            // Game-wise this makes sense, why would the player be allowed to regen
            // points faster than they can use them?
            recoveryMinusCooldownWait = new WaitForSeconds(metaData.RecoveryPeriod - metaData.Cooldown);
            IsInitialized = true;
        }

        public IEnumerator GetActionCoroutine(Func<IEnumerator> action)
        {
            if (!canPerformAction) return null;
            return DoAction(action);
        }

        private IEnumerator DoAction(Func<IEnumerator> action)
        {
            canPerformAction = false;
            points.Decrement();
            IsPerforming = true;
            yield return StartCoroutine(action());
            IsPerforming = false;

            yield return cooldownWait;
            canPerformAction = points > 0;

            yield return recoveryMinusCooldownWait;
            points.Increment();
            canPerformAction = points > 0;
        }
    }
}
