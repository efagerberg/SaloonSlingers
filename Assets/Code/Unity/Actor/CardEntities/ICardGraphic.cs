using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public interface ICardGraphic
    {
        public Card Card
        {
            get;
            set;
        }

        public Color Color { get; set; }

        public Transform transform { get; }
        public GameObject gameObject { get; }
        public T GetComponent<T>();
    }
}
