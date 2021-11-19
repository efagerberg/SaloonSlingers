
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

using GambitSimulator.Core;

namespace GambitSimulator.Unity
{
    public class CardInteractor : XRDirectInteractor
    {
        [SerializeField]
        private GameObject cardPrefab;
        [SerializeField]
        private PlayerAttributes playerAttributes;

        public override void GetValidTargets(List<XRBaseInteractable> targets)
        {
            base.GetValidTargets(targets);
            if (isSelectActive && targets.Count == 0 && playerAttributes.Deck.Count > 0)
                targets.Add(SpawnCard());
        }

        private CardInteractable SpawnCard()
        {
            Card card = playerAttributes.Deck.RemoveFromTop();
            var go = Instantiate(cardPrefab);
            var cardComponent = go.GetComponent<CardInteractable>();
            cardComponent.SetCard(card);
            return cardComponent;
        }
    }
}
