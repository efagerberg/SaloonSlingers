using System.Collections.Generic;

using UnityEngine;

using SaloonSlingers.Core;
using SaloonSlingers.Core.SlingerAttributes;
using System;

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
        [SerializeField]
        private GameRulesManager gameRulesManager;

        public void TakeDamage(int amount)
        {
            Attributes.Health -= Mathf.Max(amount, 0);
            if (Attributes.Health == 0)
                Debug.Log("Player died");
        }

        public void GameRulesChangedHandler(GameRules rules, EventArgs _)
        {
            Attributes.Hand.HandEvaluator = rules.HandEvaluator;
        }

        private void Start()
        {
            var gameRulesManagerGO = GameObject.FindGameObjectWithTag("GameRulesManager");
            gameRulesManager = gameRulesManagerGO.GetComponent<GameRulesManager>();
            Attributes = new PlayerAttributes
            {
                Deck = new Deck(numberOfCards),
                Hand = new Hand(gameRulesManager.GameRules.HandEvaluator),
                Health = startingHealth,
                Dashes = startingDashes,
                DashSpeed = startingDashSpeed,
                DashCooldown = startingDashCooldown
            };
            gameRulesManager.OnGameRulesChanged += GameRulesChangedHandler;
        }

        private void OnDisable()
        {
            gameRulesManager.OnGameRulesChanged -= GameRulesChangedHandler;
        }

        private void OnTriggerEnter(Collider hit)
        {
            if (!hit.CompareTag("Enemy")) return;
            TakeDamage(1);
            Destroy(hit.gameObject);
        }
    }
}