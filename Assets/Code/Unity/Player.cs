using System.Collections.Generic;

using UnityEngine;

using SaloonSlingers.Core;
using SaloonSlingers.Core.SlingerAttributes;

namespace SaloonSlingers.Unity
{
    public class Player : MonoBehaviour, ISlinger
    {
        public ISlingerAttributes Attributes { get; private set; }
        [SerializeField]
        private int numberOfCards = Deck.NUMBER_OF_CARDS_IN_STANDARD_DECK;
        [SerializeField]
        private int startingHealth = 5;
        [SerializeField]
        private float startingDashSpeed = 6f;
        [SerializeField]
        private int startingDashes = 3;
        [SerializeField]
        private float startingDashCooldown = 3;

        private void Awake()
        {
            Attributes = new PlayerAttributes
            {
                Deck = new Deck(numberOfCards).Shuffle(),
                Hand = new List<Card>(),
                Level = 1,
                Health = startingHealth,
                Dashes = startingDashes,
                DashSpeed = startingDashSpeed,
                DashCooldown = startingDashCooldown
            };
        }
    }
}