using UnityEngine;
using UnityEngine.Events;

namespace SaloonSlingers.Unity.Actor
{
    public abstract class Actor : MonoBehaviour
    {
        public UnityEvent<GameObject> OnKilled = new();
        public UnityEvent<GameObject> OnReset = new();
        public virtual void ResetActor() { }
    }
}
