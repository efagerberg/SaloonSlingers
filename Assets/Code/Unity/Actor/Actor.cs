using UnityEngine;
using UnityEngine.Events;

namespace SaloonSlingers.Unity.Actor
{
    public abstract class Actor : MonoBehaviour
    {
        public UnityEvent<Actor> OnKilled = new();
        public UnityEvent<Actor> OnReset = new();
        public virtual void ResetActor() { }
    }
}
