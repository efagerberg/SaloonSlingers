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
            UpdateIndicator(collider.gameObject);
            isHovering = true;
        }

        public void OnHoverExit(Collider collider)
        {
            interactabilityIndicator.Hide();
            isHovering = false;
        }

        private void UpdateIndicator(GameObject selecting)
        {
            IReadOnlyCollection<Card> hand;
            HandEvaluation evaluation;
            var projectile = GetProjectile(selecting);
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

        private HandProjectile GetProjectile(GameObject selecting)
        {
            interactabilityIndicator.transform.position = deckGraphic.TopCardTransform.position;
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
            if (!isHovering) return;

            DrawContext drawCtx = new()
            {
                AttributeRegistry = attributeRegistry,
                Deck = deckGraphic.Deck,
                Hand = sender.Cards,
                Evaluation = sender.HandEvaluation
            };
            bool canDraw = GameManager.Instance.Saloon.HouseGame.CanDraw(drawCtx);
            interactabilityIndicator.transform.position = deckGraphic.TopCardTransform.position;
            interactabilityIndicator.Indicate(canDraw);
        }

        private void OnThrown(HandProjectile sender)
        {
            if (!isHovering) return;

            DrawContext drawCtx = new()
            {
                AttributeRegistry = attributeRegistry,
                Deck = deckGraphic.Deck,
                Hand = emptyHand,
                Evaluation = GameManager.Instance.Saloon.HouseGame.Evaluate(emptyHand)
            };
            bool canDraw = GameManager.Instance.Saloon.HouseGame.CanDraw(drawCtx);
            interactabilityIndicator.transform.position = deckGraphic.TopCardTransform.position;
            interactabilityIndicator.Indicate(canDraw);
            sender.OnDraw.RemoveListener(OnDrawn);
            sender.OnThrow.RemoveListener(OnThrown);
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
