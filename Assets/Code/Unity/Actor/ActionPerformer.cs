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

        public IEnumerator GetActionCoroutine(Core.HitPoints points, ActionMetaData metaData, Func<IEnumerator> action)
        {
            if (!canPerformAction) return null;
            return DoAction(points, metaData, action);
        }

        private IEnumerator DoAction(Core.HitPoints points, ActionMetaData metaData, Func<IEnumerator> action)
        {
            canPerformAction = false;
            points.Decrement();
            IsPerforming = true;
            yield return StartCoroutine(action());
            IsPerforming = false;

            yield return new WaitForSeconds(metaData.CoolDown);
            canPerformAction = points.Value > 0;

            // Assumes the cooldown is always smaller than the recovery period
            // Game-wise this makes sense, why would the player be allowed to regen
            // points faster than they can use them.
            yield return new WaitForSeconds(metaData.PointRecoveryPeriod - metaData.CoolDown);
            points.Increment();
            canPerformAction = true;
        }
    }
}
