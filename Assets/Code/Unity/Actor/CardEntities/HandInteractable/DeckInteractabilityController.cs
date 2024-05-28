using System.Collections.Generic;

using SaloonSlingers.Core;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SaloonSlingers.Unity.Actor
{

    public class DeckInteractabilityController : MonoBehaviour
    {
        [SerializeField]
        private DeckGraphic deckGraphic;
        [SerializeField]
        private InteractabilityIndicator interactabilityIndicator;

        private ISpawner<GameObject> handInteractableSpawner;
        private IReadOnlyCollection<Card> emptyHand;
        private IReadOnlyDictionary<AttributeType, Attribute> attributeRegistry;
        private bool isHovering = false;

        public void OnHoverEnter(Collider collider)
        {
            isHovering = true;
            interactabilityIndicator.transform.position = deckGraphic.TopCardTransform.position;
            IReadOnlyCollection<Card> hand;
            HandEvaluation evaluation;
            var projectile = GetProjectile(collider.gameObject);
            if (projectile == null)
            {
                hand = emptyHand;
                evaluation = GameManager.Instance.Saloon.HouseGame.Evaluate(emptyHand);
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
            bool canDraw = GameManager.Instance.Saloon.HouseGame.CanDraw(drawCtx);
            interactabilityIndicator.Indicate(canDraw);
        }

        public void OnHoverExit(Collider collider)
        {
            interactabilityIndicator.Hide();
            isHovering = false;
        }

        private HandProjectile GetProjectile(GameObject selecting)
        {
            var interactor = selecting.GetComponent<IXRSelectInteractor>();
            if (interactor != null && interactor.isSelectActive && interactor.interactablesSelected.Count > 0)
            {
                if (interactor.interactablesSelected[0].transform.TryGetComponent(out HandProjectile projectile))
                    return projectile;
            }

            return GetComponentInChildren<HandProjectile>();
        }

        private void OnDrawn(HandProjectile sender, ICardGraphic drawn)
        {
            interactabilityIndicator.transform.position = deckGraphic.TopCardTransform.position;
            if (!isHovering) return;

            DrawContext drawCtx = new()
            {
                AttributeRegistry = attributeRegistry,
                Deck = deckGraphic.Deck,
                Hand = sender.Cards,
                Evaluation = sender.HandEvaluation
            };
            bool canDraw = GameManager.Instance.Saloon.HouseGame.CanDraw(drawCtx);
            interactabilityIndicator.Indicate(canDraw);
        }

        private void OnThrown(HandProjectile sender)
        {
            interactabilityIndicator.transform.position = deckGraphic.TopCardTransform.position;
            sender.OnDraw.RemoveListener(OnDrawn);
            sender.OnThrow.RemoveListener(OnThrown);
            if (!isHovering) return;

            DrawContext drawCtx = new()
            {
                AttributeRegistry = attributeRegistry,
                Deck = deckGraphic.Deck,
                Hand = emptyHand,
                Evaluation = GameManager.Instance.Saloon.HouseGame.Evaluate(emptyHand)
            };
            bool canDraw = GameManager.Instance.Saloon.HouseGame.CanDraw(drawCtx);
            interactabilityIndicator.Indicate(canDraw);

            // Check case where we throw but do not trigger the next hover event
            HandProjectile projectile = GetComponentInChildren<HandProjectile>();
            if (projectile != null)
            {
                projectile.OnDraw.AddListener(OnDrawn);
                projectile.OnThrow.AddListener(OnThrown);
            }
        }

        private void Awake()
        {
            emptyHand = new Card[0];
            attributeRegistry = LevelManager.Instance.Player.GetComponent<Attributes>().Registry;
        }

        private void Start()
        {
            interactabilityIndicator.transform.position = deckGraphic.TopCardTransform.position;
        }
    }
}
