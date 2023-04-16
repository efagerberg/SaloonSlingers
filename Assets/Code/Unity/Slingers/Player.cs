using SaloonSlingers.Core;
using SaloonSlingers.Core.SlingerAttributes;

using UnityEngine;

namespace SaloonSlingers.Unity.Slingers
{
    public class Player : MonoBehaviour, ISlinger
    {
        public ISlingerAttributes Attributes { get; private set; }
        [SerializeField]
        private int numberOfCards = Deck.NUMBER_OF_CARDS_IN_STANDARD_DECK;

        private void Awake()
        {
            Attributes = new PlayerAttributes(
                new Deck(numberOfCards).Shuffle()
            );
        }
    }
}