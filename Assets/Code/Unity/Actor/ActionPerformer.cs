using System;
using System.Collections;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    /// <summary>
    /// Abstracts some action to be taken that costs some amount of action points to perform.
    /// </summary>
    public class ActionPerformer : MonoBehaviour
    {
        private bool canPerformAction = true;
        protected bool IsPerforming { get; set; }

        public IEnumerator GetActionCoroutine(ActionPoints points, Func<IEnumerator> action)
        {
            if (!canPerformAction) return null;
            return DoAction(points, action);
        }

        private IEnumerator DoAction(ActionPoints points, Func<IEnumerator> action)
        {
            canPerformAction = false;
            points.Value -= 1;
            IsPerforming = true;
            yield return StartCoroutine(action());
            IsPerforming = false;

            yield return new WaitForSeconds(points.CoolDown);
            canPerformAction = points.Value > 0;

            // Assumes the cooldown is always smaller than the recovery period
            // Game-wise this makes sense, why would the player be allowed to regen
            // points faster than they can use them.
            yield return new WaitForSeconds(points.PointRecoveryPeriod - points.CoolDown);
            points.Value += 1;
            canPerformAction = true;
        }
    }
}
