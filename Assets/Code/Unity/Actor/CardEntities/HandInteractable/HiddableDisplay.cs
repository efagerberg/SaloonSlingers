using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public abstract class HiddableDisplay : MonoBehaviour
    {
        public bool IsDisplaying { get; private set; } = false;

        public virtual void Show()
        {
            IsDisplaying = true;
            UpdateContents();
        }

        public virtual void Hide()
        {
            IsDisplaying = false;
        }

        public abstract void UpdateContents();
    }
}
