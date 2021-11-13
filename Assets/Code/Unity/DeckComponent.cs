using UnityEngine;

using GambitSimulator.Core;

namespace GambitSimulator.Unity
{
    public class DeckComponent : MonoBehaviour
    {
        private Deck deck;
        void Start()
        {
            deck = new Deck(3).Shuffle();
            if (deck.Count > 0)
            {
                SpawnCard();
            }
        }

        public GameObject SpawnCard()
        {
            Card card = deck.RemoveFromTop();
            var go = (GameObject)Instantiate(Resources.Load("Prefabs/Card"));
            go.GetComponent<CardComponent>().SetCard(card);
            return go;
        }
    }
}
