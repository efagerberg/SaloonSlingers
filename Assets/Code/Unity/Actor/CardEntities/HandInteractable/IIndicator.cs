using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public interface IIndicator
    {
        public Transform transform { get; }
        public void Hide();
        public void Indicate(bool allowed);
    }
}
