using System.Collections.Generic;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class DeckGraphicCoordinator
    {
        public GameObject PeekTop(GameObject fallback = null)
        {
            return CardGraphics.Count > 0 ? CardGraphics.Peek() : fallback;
        }

        public GameObject Pop() => CardGraphics.Pop();

        public void SpawnDeck(Deck deck, Transform parent, ISpawner<GameObject> cardSpawner, float zOffset = 0.001f)
        {
            for (int i = CardGraphics.Count; i < deck.Count; i++)
            {
                var go = cardSpawner.Spawn();
                go.transform.SetParent(parent, false);
                go.transform.localPosition = GetLocalPositionOfCard(parent.localPosition, i, zOffset);
                CardGraphics.Push(go);
            }
        }

        private readonly Stack<GameObject> CardGraphics = new();

        private Vector3 GetLocalPositionOfCard(Vector3 position, int i, float zOffset)
        {
            return new(
                position.x,
                position.y,
                position.z + zOffset * i
            );
        }
    }
}
