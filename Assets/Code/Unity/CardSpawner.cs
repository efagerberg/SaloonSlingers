using UnityEngine;

using GambitSimulator.Core;

namespace GambitSimulator.Unity
{
    public class CardSpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject cardPrefab;
        [SerializeField]
        private PlayerAttributes playerAttributes;

        public bool TrySpawnCard(Vector3 spawnPosition, out CardInteractable cardInteractable)
        {
            cardInteractable = null;
            if (playerAttributes.Deck.Count == 0) return false;

            Card card = playerAttributes.Deck.RemoveFromTop();
            var go = Instantiate(cardPrefab, spawnPosition, Quaternion.identity);
            var cardComponent = go.GetComponent<CardInteractable>();
            cardComponent.SetCard(card);
            cardInteractable = cardComponent;
            return true;
        }
    }
}
