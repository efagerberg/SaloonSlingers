using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class HandInteractablePlacer : MonoBehaviour
    {
        private DeckGraphic deckGraphic;
        private ISpawner<GameObject> handInteractableSpawner;
        private GameObject placed;
        private DrawContext firstDrawContext;

        private void Awake()
        {
            deckGraphic = GetComponent<DeckGraphic>();
            var emptyHand = new Card[] { };
            firstDrawContext = new()
            {
                AttributeRegistry = LevelManager.Instance.Player.GetComponent<Attributes>().Registry,
                Deck = deckGraphic.Deck,
                Hand = emptyHand,
                Evaluation = GameManager.Instance.Saloon.HouseGame.Evaluate(emptyHand)
            };
        }

        private void Start()
        {
            handInteractableSpawner = LevelManager.Instance.HandInteractableSpawner;

            if (!deckGraphic.CanDraw ||
                !GameManager.Instance.Saloon.HouseGame.CanDraw(firstDrawContext)) return;

            PlaceOnTop(deckGraphic.TopCardTransform, SpawnInteractable());
        }

        private void Update()
        {
            if (!deckGraphic.CanDraw && placed != null)
            {
                var unusableProjectile = placed.GetComponent<HandProjectileActor>();
                unusableProjectile.Kill();
            }
        }

        private void PlaceOnTop(Transform topCardTransform, GameObject cardHandGO)
        {
            cardHandGO.transform.SetPositionAndRotation(
                topCardTransform.position, topCardTransform.rotation
            );
            cardHandGO.transform.SetParent(transform);
            placed = cardHandGO;
        }

        private void HandInteractableHeldHandler(GameObject sender)
        {
            if (sender == placed) placed = null;

            if (!deckGraphic.CanDraw ||
                !GameManager.Instance.Saloon.HouseGame.CanDraw(firstDrawContext) ||
                placed != null) return;

            PlaceOnTop(deckGraphic.TopCardTransform, SpawnInteractable());
        }

        private void HandleInteractableDeath(GameObject sender)
        {
            var instance = sender as GameObject;
            var projectile = instance.GetComponent<HandProjectileActor>();
            projectile.OnPickup.RemoveListener(HandInteractableHeldHandler);
            projectile.OnKilled.RemoveListener(HandleInteractableDeath);
        }

        private GameObject SpawnInteractable()
        {
            GameObject spawned = handInteractableSpawner.Spawn();
            HandProjectileActor projectile = spawned.GetComponent<HandProjectileActor>();
            projectile.OnPickup.AddListener(HandInteractableHeldHandler);
            projectile.OnKilled.AddListener(HandleInteractableDeath);
            ControllerSwapper swapper = spawned.GetComponent<ControllerSwapper>();
            swapper.SetController(ControllerTypes.PLAYER);
            return spawned;
        }
    }
}
