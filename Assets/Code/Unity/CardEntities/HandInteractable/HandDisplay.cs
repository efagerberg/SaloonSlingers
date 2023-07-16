using System.Collections.Generic;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.CardEntities;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public abstract class HandDisplay : MonoBehaviour
    {
        public bool IsDisplaying { get; private set; } = false;

        public virtual void Show() => IsDisplaying = true;

        public virtual void Hide() => IsDisplaying = false;

        protected abstract void UpdateContents(HandEvaluation evaluation);

        protected HandProjectile projectile;

        private void Update()
        {
            if (!IsDisplaying || projectile == null || projectile.Cards.Count <= 0 || lastCards == projectile.Cards)
                return;

            if (projectile.HandEvaluation.Name == HandNames.NONE) return;

            UpdateContents(projectile.HandEvaluation);

            lastCards = projectile.Cards;
        }

        private IList<Card> lastCards;
    }
}
