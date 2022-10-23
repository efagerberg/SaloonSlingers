using System.Collections.Generic;

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
        private int startingHealth = 5;
        [SerializeField]
        private float startingDashSpeed = 6f;
        [SerializeField]
        private int startingDashes = 3;
        [SerializeField]
        private float startingDashCooldown = 3;
        [SerializeField]
        private Handedness defaultHandedness = Handedness.RIGHT;

        private void Awake()
        {
            Attributes = new PlayerAttributes
            {
                Deck = new Deck(numberOfCards).Shuffle(),
                Level = 1,
                Health = startingHealth,
                Dashes = startingDashes,
                DashSpeed = startingDashSpeed,
                DashCooldown = startingDashCooldown,
                Handedness = defaultHandedness
            };
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