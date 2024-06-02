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
        /// Kills the actor, disabling it and moving it back to the actor pool.
        /// 
        /// Sometimes other objects need to react to the death of an actor.
        /// To facilitate this, this function accepts a delay flag.
        /// </summary>
        public void Kill(bool delay = false)
        {
            if (delay) StartCoroutine(nameof(DelayDeath));
            else OnKilled.Invoke(this);
        }

        private IEnumerator DelayDeath()
        {
            var delay = deathDelaySeconds > 0 ? new WaitForSeconds(deathDelaySeconds) : null;
            yield return delay;
            OnKilled.Invoke(this);
        }
    }
}
