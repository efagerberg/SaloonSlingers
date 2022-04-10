using UnityEngine;

using SaloonSlingers.Core;

namespace SaloonSlingers.Unity
{
    public interface ICardGraphic
    {
        public Card Card
        {
            get;
            set;
        }

        public Transform transform { get; }
        public GameObject gameObject { get; }
        public T GetComponent<T>();
        public void SetGraphics(Card card);
    }
}
