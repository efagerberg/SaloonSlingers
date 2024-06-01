using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public interface IPeeker<T>
    {
        public T Peek();
    }

    public interface IDeckGraphic : IPeeker<Transform>, ISpawner<GameObject>
    {
        public bool CanDraw { get; }
        public Deck Deck { get; }
        public T GetComponentInChildren<T>();
    }
}
