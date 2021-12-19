using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

using SaloonSlingers.Core;

namespace SaloonSlingers.Unity
{
    public class Player : MonoBehaviour
    {
        public PlayerAttributes Attributes;
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

        public void TakeDamage(int amount)
        {
            Attributes.Health -= Mathf.Max(amount, 0);
            if (Attributes.Health == 0)
                Debug.Log("Player died");
        }

        private void Awake()
        {
            Attributes = new PlayerAttributes
            {
                Deck = new Deck(numberOfCards),
                Health = startingHealth,
                Dashes = startingDashes,
                DashSpeed = startingDashSpeed,
                DashCooldown = startingDashCooldown
            };
        }

        private void OnTriggerEnter(Collider hit)
        {
            if (!hit.CompareTag("Enemy")) return;
            TakeDamage(1);
            Destroy(hit.gameObject);
        }
    }
}