using System.Collections.Generic;

using SaloonSlingers.Core;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SaloonSlingers.Unity.Actor
{
    public class DeckInteractability
    {
        public ICardGame Game { get; set; }

        private readonly IDeckGraphic deckGraphic;
        private readonly IIndicator indicator;
        private readonly ISpawner<GameObject> handInteractableSpawner;
        private readonly IReadOnlyCollection<Card> emptyHand = new Card[0];
        private readonly IReadOnlyDictionary<AttributeType, Attribute> attributeRegistry;
        private bool isHovering = false;

        public DeckInteractability(IIndicator indicator,
                                   ISpawner<GameObject> handInteractableSpawner,
                                   IReadOnlyDictionary<AttributeType, Attribute> attributeRegistry,
                                   IDeckGraphic deckGraphic,
                                   ICardGame game)
        {
            Game = game;
            this.indicator = indicator;
            this.handInteractableSpawner = handInteractableSpawner;
            this.attributeRegistry = attributeRegistry;
            this.deckGraphic = deckGraphic;
            indicator.transform.position = deckGraphic.Peek().position;
        }

        public void OnHoverEnter(IXRSelectInteractor interactor)
        {
            isHovering = true;
            indicator.transform.position = deckGraphic.Peek().position;
            IReadOnlyCollection<Card> hand;
            HandEvaluation evaluation;
            var projectile = GetHandProjectileFromHover(interactor);
            if (projectile == null)
            {
                hand = emptyHand;
                evaluation = Game.Evaluate(emptyHand);
            }
            else
            {
                projectile.OnDraw.AddListener(OnDrawn);
                projectile.OnThrow.AddListener(OnThrown);
                hand = projectile.Cards;
                evaluation = projectile.HandEvaluation;
            }

            DrawContext drawCtx = new()
            {
                AttributeRegistry = attributeRegistry,
                Deck = deckGraphic.Deck,
                Hand = hand,
                Evaluation = evaluation
            };
            bool canDraw = Game.CanDraw(drawCtx);
            indicator.Indicate(canDraw);
        }

        public void OnHoverExit()
        {
            indicator.Hide();
            isHovering = false;
        }

        private HandProjectile GetHandProjectileFromHover(IXRSelectInteractor interactor)
        {
            if (interactor != null && interactor.isSelectActive && interactor.interactablesSelected.Count > 0)
            {
                if (interactor.interactablesSelected[0].transform.TryGetComponent(out HandProjectile projectile))
                    return projectile;
            }

            return deckGraphic.GetComponentInChildren<HandProjectile>();
        }

        private void OnDrawn(HandProjectile sender, ICardGraphic drawn)
        {
            indicator.transform.position = deckGraphic.Peek().position;
            if (!isHovering) return;

            DrawContext drawCtx = new()
            {
                AttributeRegistry = attributeRegistry,
                Deck = deckGraphic.Deck,
                Hand = sender.Cards,
                Evaluation = sender.HandEvaluation
            };
            bool canDraw = Game.CanDraw(drawCtx);
            indicator.Indicate(canDraw);
        }

        private void OnThrown(HandProjectile throwing)
        {
            indicator.transform.position = deckGraphic.Peek().position;
            throwing.OnDraw.RemoveListener(OnDrawn);
            throwing.OnThrow.RemoveListener(OnThrown);
            if (!isHovering) return;

            DrawContext drawCtx = new()
            {
                AttributeRegistry = attributeRegistry,
                Deck = deckGraphic.Deck,
                Hand = emptyHand,
                Evaluation = Game.Evaluate(emptyHand)
            };
            bool canDraw = Game.CanDraw(drawCtx);
            indicator.Indicate(canDraw);

            // Check case where we throw but do not trigger the next hover event
            HandProjectile projectile = deckGraphic.GetComponentInChildren<HandProjectile>();
            if (projectile != null)
            {
                projectile.OnDraw.AddListener(OnDrawn);
                projectile.OnThrow.AddListener(OnThrown);
            }
        }
    }
}
