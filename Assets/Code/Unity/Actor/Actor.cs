using System.Collections;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace SaloonSlingers.Unity.Actor
{
    public class Actor : MonoBehaviour
    {
        [FormerlySerializedAs("OnKilled")]
        public UnityEvent<Actor> Killed = new();
        [FormerlySerializedAs("OnReset")]
        public UnityEvent<Actor> Reset = new();

        [SerializeField]
        protected float deathDelaySeconds = 0f;

        public virtual void ResetActor()
        {
            Reset.Invoke(this);
        }

        /// <summary>
        /// Kills the actor, disabling it and moving it back to the actor pool.
        /// 
        /// Sometimes other objects need to react to the death of an actor.
        /// To facilitate this, this function accepts a delay flag.
        /// </summary>
        public void Kill(bool delay = false)
        {
            if (delay) StartCoroutine(DelayDeath());
            else Killed.Invoke(this);
        }

        private IEnumerator DelayDeath()
        {
            var delay = deathDelaySeconds > 0 ? new WaitForSeconds(deathDelaySeconds) : null;
            yield return delay;
            Killed.Invoke(this);
        }
    }
}
