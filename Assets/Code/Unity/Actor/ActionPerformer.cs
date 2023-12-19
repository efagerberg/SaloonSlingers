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

        public void Initialize(Points points, ActionMetaData metaData)
        {
            Points = points;
            MetaData = metaData;
            canPerformAction &= Points > 0;
        }

        public IEnumerator GetActionCoroutine(Func<IEnumerator> action)
        {
            if (!canPerformAction) return null;
            return DoAction(action);
        }

        private IEnumerator DoAction(Func<IEnumerator> action)
        {
            canPerformAction = false;
            Points.Decrement();
            IsPerforming = true;
            yield return StartCoroutine(action());
            IsPerforming = false;

            yield return new WaitForSeconds(MetaData.Cooldown);
            canPerformAction = Points.Value > 0;

            // Assumes the cooldown is always smaller than the recovery period
            // Game-wise this makes sense, why would the player be allowed to regen
            // points faster than they can use them?
            yield return new WaitForSeconds(MetaData.RecoveryPeriod - MetaData.Cooldown);
            Points.Increment();
            canPerformAction = true;
        }
    }
}
