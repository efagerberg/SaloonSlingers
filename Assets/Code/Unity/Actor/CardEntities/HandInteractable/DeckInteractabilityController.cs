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

        private DeckInteractability deckInteractability;

        public void OnHoverEnter(Collider collider)
        {
            var interactor = collider.gameObject.GetComponent<IXRSelectInteractor>();
            deckInteractability.OnHoverEnter(interactor);
        }

        public void OnHoverExit(Collider collider)
        {
            deckInteractability.OnHoverExit();
        }

        private void Start()
        {
            deckInteractability = new(interactabilityIndicator,
                                      LevelManager.Instance.HandInteractableSpawner,
                                      LevelManager.Instance.Player.GetComponent<Attributes>().Registry,
                                      deckGraphic,
                                      GameManager.Instance.Saloon.HouseGame);
        }
    }
}
