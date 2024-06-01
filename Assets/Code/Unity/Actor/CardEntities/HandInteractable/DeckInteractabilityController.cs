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
            var projectileInvolved = GetHandProjectileFromHover(interactor);
            if (projectileInvolved != null)
            {
                projectileInvolved.OnDraw.AddListener(OnDrawn);
                projectileInvolved.OnThrow.AddListener(OnThrown);
            }

            var result = detector.OnHoverEnter(projectileInvolved);
            UpdateIndicator(result);
        }

        public void OnHoverExit(Collider collider)
        {
            var result = detector.OnHoverExit();
            UpdateIndicator(result);
        }

        private void OnDrawn(HandProjectile sender, ICardGraphic card)
        {
            var result = detector.OnDrawn(sender);
            UpdateIndicator(result);
        }

        private void OnThrown(HandProjectile sender)
        {
            sender.OnDraw.RemoveListener(OnDrawn);
            sender.OnThrow.RemoveListener(OnThrown);

            // Check case where we throw but do not trigger the next hover event
            HandProjectile nextProjectile = deckGraphic.GetComponentInChildren<HandProjectile>();
            if (nextProjectile != null)
            {
                nextProjectile.OnDraw.AddListener(OnDrawn);
                nextProjectile.OnThrow.AddListener(OnThrown);
            }
            UpdateIndicator(nextProjectile);
        }

        private void Start()
        {
            detector = new(LevelManager.Instance.Player.GetComponent<Attributes>().Registry,
                                      deckGraphic.Deck,
                                      GameManager.Instance.Saloon.HouseGame);
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

        private void UpdateIndicator(bool? interaxctabilityMode)
        {
            if (interaxctabilityMode == null) interactabilityIndicator.Hide();
            else
            {
                interactabilityIndicator.transform.position = deckGraphic.Peek().transform.position;
                interactabilityIndicator.Indicate(interaxctabilityMode.Value);
            }
        }
    }
}
