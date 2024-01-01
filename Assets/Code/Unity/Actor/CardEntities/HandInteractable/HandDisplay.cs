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
            if (projectile == null) return;

            UpdateContents(projectile.HandEvaluation);
        }

        public virtual void Hide()
        {
            IsDisplaying = false;
            projectile = null;
        }

        protected abstract void UpdateContents(HandEvaluation evaluation);

        protected HandProjectile projectile;

        private void Update()
        {
            if (!IsDisplaying) return;

            // Allows for hiding hand display even if it is set to be shown, but there is no projectile set yet.
            var currentEvaluation = projectile != null ? projectile.HandEvaluation : new HandEvaluation(HandNames.NONE, 0);
            if (!lastEvaluation.HasValue || !lastEvaluation.Value.Equals(currentEvaluation))
                UpdateContents(currentEvaluation);
            lastEvaluation = currentEvaluation;
        }

        private HandEvaluation? lastEvaluation;
    }
}
