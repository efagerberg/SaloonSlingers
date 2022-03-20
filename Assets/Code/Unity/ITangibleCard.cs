using UnityEngine;

using SaloonSlingers.Core;

namespace SaloonSlingers.Unity
{
    public interface ITangibleCard
    {
        public Card Card
        {
            get;
            set;
        }

        public Transform transform { get; }
        public GameObject gameObject { get; }
        public T GetComponent<T>();
    }
}
