using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{

    public class DeckInteractabilityController : MonoBehaviour
    {
        [SerializeField]
        private DeckGraphic deckGraphic;
        [SerializeField]
        private InteractabilityIndicator interactabilityIndicator;

        private DeckInteractabilityCoordinator coordinator;

        public void OnHoverEnter(Collider collider)
        {
            coordinator.OnHoverEnter(collider.gameObject, GameManager.Instance.Saloon.HouseGame);
        }

        public void OnHoverExit(Collider collider)
        {
            coordinator.OnHoverExit();
        }

        private void Start()
        {
            coordinator = new(interactabilityIndicator,
                              LevelManager.Instance.HandInteractableSpawner,
                              LevelManager.Instance.Player.GetComponent<Attributes>().Registry,
                              deckGraphic);
        }
    }
}
