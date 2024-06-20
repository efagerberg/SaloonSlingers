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

        private DeckInteractabilityDetector detector;

        public void OnHoverEnter(Collider collider)
        {
            var interactor = collider.gameObject.GetComponent<IXRSelectInteractor>();
            var interactableInvolved = GetInteractable(interactor);
            if (interactableInvolved != null)
            {
                interactableInvolved.Drawn.AddListener(OnDrawn);
                var projectile = interactableInvolved.GetComponent<Projectile>();
                projectile.Thrown.AddListener(OnThrown);
            }

            var result = detector.OnHoverEnter(interactableInvolved);
            UpdateIndicator(result);
        }

        public void OnHoverExit(Collider collider)
        {
            var result = detector.OnHoverExit();
            UpdateIndicator(result);
        }

        private void OnDrawn(CardHand sender, ICardGraphic card)
        {
            var result = detector.OnDrawn(sender);
            UpdateIndicator(result, false);
        }

        private void OnThrown(Projectile sender)
        {
            var hand = sender.GetComponent<CardHand>();
            hand.Drawn.RemoveListener(OnDrawn);
            sender.Thrown.RemoveListener(OnThrown);

            // Check case where we throw but do not trigger the next hover event
            CardHand nextHand = deckGraphic.GetComponentInChildren<CardHand>();
            Projectile nextProjectile = nextHand.GetComponentInChildren<Projectile>();
            if (nextHand != null)
            {
                nextHand.Drawn.AddListener(OnDrawn);
                nextProjectile.Thrown.AddListener(OnThrown);
            }
            var result = detector.OnThrown(nextHand);
            UpdateIndicator(result);
        }

        private void Start()
        {
            detector = new(LevelManager.Instance.Player.GetComponent<Attributes>().Registry,
                                      deckGraphic.Deck,
                                      GameManager.Instance.Saloon.HouseGame);
        }

        private CardHand GetInteractable(IXRSelectInteractor interactor)
        {
            if (interactor != null && interactor.isSelectActive && interactor.interactablesSelected.Count > 0)
            {
                if (interactor.interactablesSelected[0].transform.TryGetComponent(out CardHand projectile))
                    return projectile;
            }

            return deckGraphic.GetComponentInChildren<CardHand>();
        }

        private void UpdateIndicator(bool? interaxctabilityMode, bool playSound = true)
        {
            if (interaxctabilityMode == null) interactabilityIndicator.Hide();
            else
            {
                interactabilityIndicator.transform.position = deckGraphic.Peek().transform.position;
                interactabilityIndicator.Indicate(interaxctabilityMode.Value, playSound);
            }
        }
    }
}
