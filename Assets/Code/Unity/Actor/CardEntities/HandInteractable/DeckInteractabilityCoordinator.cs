using System.Collections.Generic;

using SaloonSlingers.Core;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SaloonSlingers.Unity.Actor
{
    public class DeckInteractabilityCoordinator
    {
        private readonly DeckGraphic deckGraphic;
        private readonly IIndicator indicator;
        private readonly ISpawner<GameObject> handInteractableSpawner;
        private readonly IReadOnlyCollection<Card> emptyHand = new Card[0];
        private readonly IReadOnlyDictionary<AttributeType, Attribute> attributeRegistry;
        private bool isHovering = false;

        public DeckInteractabilityCoordinator(IIndicator indicator,
                                              ISpawner<GameObject> handInteractableSpawner,
                                              IReadOnlyDictionary<AttributeType, Attribute> attributeRegistry,
                                              DeckGraphic deckGraphic)
        {
            this.indicator = indicator;
            this.handInteractableSpawner = handInteractableSpawner;
            this.attributeRegistry = attributeRegistry;
            this.deckGraphic = deckGraphic;
            indicator.transform.position = deckGraphic.TopCardTransform.position;
        }

        public void OnHoverEnter(GameObject interactorGO, CardGame game)
        {
            isHovering = true;
            indicator.transform.position = deckGraphic.TopCardTransform.position;
            IReadOnlyCollection<Card> hand;
            HandEvaluation evaluation;
            var projectile = GetHandProjectileFromHover(interactorGO);
            if (projectile == null)
            {
                hand = emptyHand;
                evaluation = game.Evaluate(emptyHand);
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
            bool canDraw = game.CanDraw(drawCtx);
            indicator.Indicate(canDraw);
        }

        public void OnHoverExit()
        {
            indicator.Hide();
            isHovering = false;
        }

        private HandProjectile GetHandProjectileFromHover(GameObject interactorGO)
        {
            var interactor = interactorGO.GetComponent<IXRSelectInteractor>();
            if (interactor != null && interactor.isSelectActive && interactor.interactablesSelected.Count > 0)
            {
                if (interactor.interactablesSelected[0].transform.TryGetComponent(out HandProjectile projectile))
                    return projectile;
            }

            return deckGraphic.GetComponentInChildren<HandProjectile>();
        }

        public void OnDrawn(HandProjectile sender, ICardGraphic drawn)
        {
            indicator.transform.position = deckGraphic.TopCardTransform.position;
            if (!isHovering) return;

            DrawContext drawCtx = new()
            {
                AttributeRegistry = attributeRegistry,
                Deck = deckGraphic.Deck,
                Hand = sender.Cards,
                Evaluation = sender.HandEvaluation
            };
            bool canDraw = GameManager.Instance.Saloon.HouseGame.CanDraw(drawCtx);
            indicator.Indicate(canDraw);
        }

        public void OnThrown(HandProjectile throwing)
        {
            indicator.transform.position = deckGraphic.TopCardTransform.position;
            throwing.OnDraw.RemoveListener(OnDrawn);
            throwing.OnThrow.RemoveListener(OnThrown);
            if (!isHovering) return;

            DrawContext drawCtx = new()
            {
                AttributeRegistry = attributeRegistry,
                Deck = deckGraphic.Deck,
                Hand = emptyHand,
                Evaluation = GameManager.Instance.Saloon.HouseGame.Evaluate(emptyHand)
            };
            bool canDraw = GameManager.Instance.Saloon.HouseGame.CanDraw(drawCtx);
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
