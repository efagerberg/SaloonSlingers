using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public abstract class HandDisplay : MonoBehaviour
    {
        public bool IsDisplaying { get; private set; } = false;

        public virtual void Show()
        {
            IsDisplaying = true;
            UpdateContents(projectile.HandEvaluation);
        }

        public virtual void Hide() => IsDisplaying = false;

        protected abstract void UpdateContents(HandEvaluation evaluation);

        protected HandProjectile projectile;

        private void Update()
        {
            if (!IsDisplaying || projectile == null ||
                projectile.Cards.Count <= 0 || lastEvaluation.Equals(projectile.HandEvaluation))
                return;

            if (projectile.HandEvaluation.Name == HandNames.NONE) return;

            UpdateContents(projectile.HandEvaluation);

            lastEvaluation = projectile.HandEvaluation;
        }

        private HandEvaluation lastEvaluation;
    }
}
