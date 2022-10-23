using UnityEngine;
using UnityEngine.XR;

using SaloonSlingers.Core;
using SaloonSlingers.Core.SlingerAttributes;

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
        [SerializeField]
        private Handedness defaultHandedness = Handedness.RIGHT;

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
                ),
                defaultHandedness
            );
        }

        private void OnEnable() => InputDevices.deviceConnected += SetHandedness;

        private void OnDisable() => InputDevices.deviceConnected -= SetHandedness;

        private void SetHandedness(InputDevice device) => Attributes.Handedness = GetHandedness(device);

        private Handedness GetHandedness(InputDevice device)
        {
            if (device.TryGetFeatureValue(CommonUsages.primaryButton, out _))
            {
                switch (device.name.ToLower())
                {
                    case "left":
                        return Handedness.LEFT;
                    case "right":
                        return Handedness.RIGHT;
                }
            }
            return defaultHandedness;
        }
    }
}