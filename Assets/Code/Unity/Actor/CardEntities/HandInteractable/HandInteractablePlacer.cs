using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class HandInteractablePlacer : MonoBehaviour
    {
        private DeckGraphic deckGraphic;
        private ISpawner<GameObject> handInteractableSpawner;
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

        private void PlaceOnTop(Transform topCardTransform, GameObject cardHandGO)
        {
            cardHandGO.transform.SetPositionAndRotation(
                topCardTransform.position, topCardTransform.rotation
            );
            cardHandGO.transform.SetParent(transform);
        }

        private void OnThrow(Projectile sender)
        {
            if (!deckGraphic.CanDraw ||
                !GameManager.Instance.Saloon.HouseGame.CanDraw(firstDrawContext))
                return;

            PlaceOnTop(deckGraphic.Peek(), SpawnInteractable());
        }

        private void OnKilled(Actor sender)
        {
            var hand = sender.GetComponent<CardHand>();
            var projectile = sender.GetComponent<Projectile>();
            projectile.Thrown.RemoveListener(OnThrow);
            sender.Killed.RemoveListener(OnKilled);
        }

        private GameObject SpawnInteractable()
        {
            GameObject spawned = handInteractableSpawner.Spawn();
            Actor actor = spawned.GetComponent<Actor>();
            CardHand hand = spawned.GetComponent<CardHand>();
            hand.InitialEvaluate(GameManager.Instance.Saloon.HouseGame);
            Projectile projectile = hand.GetComponent<Projectile>();
            projectile.Thrown.AddListener(OnThrow);
            actor.Killed.AddListener(OnKilled);
            ControllerSwapper swapper = spawned.GetComponent<ControllerSwapper>();
            swapper.SetController(ControllerTypes.PLAYER);
            return spawned;
        }
    }
}
