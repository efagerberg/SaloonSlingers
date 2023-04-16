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
        [SerializeField]
        private uint startingHitPoints = 5;
        [SerializeField]
        private float startingDashSpeed = 6f;
        [SerializeField]
        private uint startingDashes = 3;
        [SerializeField]
        private float startingDashCooldown = 3;
        [SerializeField]
        private float startingDashDuration = 0.25f;
        [SerializeField]
        private float startingPointRecoveryPeriod = 1f;

        private void Awake()
        {
            Attributes = new PlayerAttributes(
                new Deck(numberOfCards).Shuffle(),
                new Points(startingHitPoints),
                new Dash(
                    startingDashes,
                    startingDashSpeed,
                    startingDashDuration,
                    startingDashCooldown,
                    startingPointRecoveryPeriod
                )
            );
        }
    }
}