using System.Collections.Generic;

using SaloonSlingers.Core;

namespace SaloonSlingers.Unity.Actor
{
    public class DeckInteractabilityDetector
    {
        public ICardGame Game { get; set; }

        private readonly Deck deck;
        private readonly IReadOnlyCollection<Card> emptyHand = new Card[0];
        private readonly IReadOnlyDictionary<AttributeType, Attribute> attributeRegistry;
        private bool isHovering = false;

        public DeckInteractabilityDetector(IReadOnlyDictionary<AttributeType, Attribute> attributeRegistry,
                                   Deck deck,
                                   ICardGame game)
        {
            Game = game;
            this.attributeRegistry = attributeRegistry;
            this.deck = deck;
        }

        public bool? OnHoverEnter(HandProjectile projectile)
        {
            isHovering = true;
            IReadOnlyCollection<Card> hand;
            HandEvaluation evaluation;
            if (projectile == null)
            {
                hand = emptyHand;
                evaluation = Game.Evaluate(emptyHand);
            }
            else
            {
                hand = projectile.Cards;
                evaluation = projectile.HandEvaluation;
            }

            return Detect(hand, evaluation);
        }

        public bool? OnHoverExit()
        {
            isHovering = false;
            return null;
        }

        public bool? OnDrawn(HandProjectile projectile)
        {
            if (projectile == null) return null;

            return Detect(projectile.Cards, projectile.HandEvaluation);
        }

        public bool? OnThrown(HandProjectile nextProjectile)
        {
            if (nextProjectile == null) return null;

            return Detect(nextProjectile.Cards, nextProjectile.HandEvaluation);
        }

        private bool? Detect(IReadOnlyCollection<Card> hand, HandEvaluation evaluation)
        {
            if (!isHovering) return null;

            DrawContext drawCtx = new()
            {
                AttributeRegistry = attributeRegistry,
                Deck = deck,
                Hand = hand,
                Evaluation = evaluation
            };
            return Game.CanDraw(drawCtx);
        }
    }
}
