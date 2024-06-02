using System.Collections;

using UnityEngine;
using UnityEngine.Events;

namespace SaloonSlingers.Unity.Actor
{
    public class Actor : MonoBehaviour
    {
        public UnityEvent<Actor> OnKilled = new();
        public UnityEvent<Actor> OnReset = new();

        [SerializeField]
        protected float deathDelaySeconds = 0f;

        public virtual void ResetActor()
        {
            OnReset.Invoke(this);
        }

        /// <summary>
        /// Sometimes we want other objects to react to the death of an actor.
        /// This requires the object to live for an extra frame.
        /// </summary>
        protected IEnumerator DelayDeath()
        {
            yield return new WaitForSeconds(deathDelaySeconds);
            OnKilled?.Invoke(this);
        }
    }
}
