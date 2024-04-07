using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public abstract class HandDisplay : MonoBehaviour
    {
        public bool IsDisplaying { get; private set; } = false;

        public virtual void Show()
        {
            IsDisplaying = true;
            if (projectile == null) return;

            UpdateContents();
        }

        public virtual void Hide()
        {
            IsDisplaying = false;
        }

        public abstract void UpdateContents();

        protected HandProjectile projectile;
    }
}
