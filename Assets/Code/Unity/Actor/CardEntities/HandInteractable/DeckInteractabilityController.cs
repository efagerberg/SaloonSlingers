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

        public void OnHoverEnter(Collider collider)
        {
            UpdateIndicator(collider.gameObject);
        }

        public void OnHoverExit(Collider collider)
        {
            interactabilityIndicator.Hide();
        }

        private void UpdateIndicator(GameObject collidingObject)
        {
            interactabilityIndicator.transform.position = deckGraphic.TopCardTransform.position;
            var baseInteractor = collidingObject.GetComponent<IXRSelectInteractor>();
            IReadOnlyCollection<Card> hand;
            HandEvaluation evaluation;
            HandProjectile projectile;
            if (baseInteractor != null && baseInteractor.isSelectActive && baseInteractor.interactablesSelected.Count > 0)
            {
                if (!baseInteractor.interactablesSelected[0].transform.TryGetComponent(out projectile))
                {
                    hand = emptyHand;
                    evaluation = GameManager.Instance.Saloon.HouseGame.Evaluate(emptyHand);
                    projectile = gameObject.GetComponentInChildren<HandProjectile>();
                    projectile.OnDraw.AddListener(OnDrawn);
                    projectile.OnThrow.AddListener(OnThrown);
                }
                hand = projectile.Cards;
                evaluation = projectile.HandEvaluation;
            }
            else
            {
                hand = emptyHand;
                evaluation = GameManager.Instance.Saloon.HouseGame.Evaluate(emptyHand);
                projectile = gameObject.GetComponentInChildren<HandProjectile>();
                projectile.OnDraw.AddListener(OnDrawn);
                projectile.OnThrow.AddListener(OnThrown);
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

        private void OnDrawn(GameObject sender, ICardGraphic drawn)
        {
            HandProjectile projectile = sender.GetComponent<HandProjectile>();
            DrawContext drawCtx = new()
            {
                AttributeRegistry = attributeRegistry,
                Deck = deckGraphic.Deck,
                Hand = projectile.Cards,
                Evaluation = projectile.HandEvaluation
            };
            bool canDraw = GameManager.Instance.Saloon.HouseGame.CanDraw(drawCtx);
            interactabilityIndicator.transform.position = deckGraphic.TopCardTransform.position;
            interactabilityIndicator.Indicate(canDraw);
        }

        private void OnThrown(GameObject sender)
        {
            HandProjectile projectile = sender.GetComponent<HandProjectile>();
            interactabilityIndicator.Hide();
            projectile.OnDraw.RemoveListener(OnDrawn);
            projectile.OnThrow.RemoveListener(OnThrown);
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
