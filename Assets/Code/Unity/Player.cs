using UnityEngine;

using GambitSimulator.Core;

namespace GambitSimulator.Unity
{
    public class Player : MonoBehaviour
    {
        public PlayerAttributes Attributes;
        [SerializeField]
        private int numberOfCards = Deck.NUMBER_OF_CARDS_IN_STANDARD_DECK;

        private void Start()
        {
            Attributes = new PlayerAttributes(numberOfCards);
        }
    }
}
