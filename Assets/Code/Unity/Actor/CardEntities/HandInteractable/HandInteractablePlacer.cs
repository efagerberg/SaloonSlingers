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

            PlaceOnTop(deckGraphic.Peek(), SpawnInteractable());
        }

        private void Update()
        {
            if (!deckGraphic.CanDraw && placed != null)
            {
                var unusavle = placed.GetComponent<Actor>();
                unusavle.Kill();
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

        private void HandInteractableThrowHandler(HandProjectile sender)
        {
            if (sender.gameObject == placed) placed = null;

            if (!deckGraphic.CanDraw ||
                !GameManager.Instance.Saloon.HouseGame.CanDraw(firstDrawContext) ||
                placed != null) return;

            PlaceOnTop(deckGraphic.Peek(), SpawnInteractable());
        }

        private void HandleInteractableDeath(Actor sender)
        {
            var projectile = sender.GetComponent<HandProjectile>();
            projectile.OnThrow.RemoveListener(HandInteractableThrowHandler);
            sender.OnKilled.RemoveListener(HandleInteractableDeath);
        }

        private GameObject SpawnInteractable()
        {
            GameObject spawned = handInteractableSpawner.Spawn();
            Actor actor = spawned.GetComponent<Actor>();
            HandProjectile projectile = spawned.GetComponent<HandProjectile>();
            projectile.InitialEvaluate(GameManager.Instance.Saloon.HouseGame);
            projectile.OnThrow.AddListener(HandInteractableThrowHandler);
            actor.OnKilled.AddListener(HandleInteractableDeath);
            ControllerSwapper swapper = spawned.GetComponent<ControllerSwapper>();
            swapper.SetController(ControllerTypes.PLAYER);
            return spawned;
        }
    }
}
