using System;
using System.Collections.Generic;

using SaloonSlingers.Core;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SaloonSlingers.Unity.Actor
{

    public class HandInteractablePlacer : MonoBehaviour
    {
        public static readonly IReadOnlyCollection<Card> EMPTY_HAND = new List<Card>();

        [SerializeField]
        private XRBaseInteractable hoverInteractable;

        private DeckGraphic deckGraphic;
        private ISpawner<GameObject> handInteractableSpawner;
        private GameObject placed;
        private DrawContext firstDrawContext;

        private void Awake()
        {
            deckGraphic = GetComponent<DeckGraphic>();
            firstDrawContext = new()
            {
                AttributeRegistry = LevelManager.Instance.Player.GetComponent<Attributes>().Registry,
                Deck = deckGraphic.Deck,
                Hand = EMPTY_HAND,
                Evaluation = GameManager.Instance.Saloon.HouseGame.Evaluate(EMPTY_HAND)
            };
        }

        private void Start()
        {
            handInteractableSpawner = LevelManager.Instance.HandInteractableSpawner;

            PlaceOnTop(deckGraphic.TopCardTransform, SpawnInteractable);
        }

        private void Update()
        {
            if (!deckGraphic.CanDraw && placed != null)
            {
                var unusableProjectile = placed.GetComponent<HandProjectile>();
                unusableProjectile.Kill();
            }
        }

        private void PlaceOnTop(Transform topCardTransform, Func<GameObject> spawnFunc)
        {
            if (deckGraphic.CanDraw &&
                GameManager.Instance.Saloon.HouseGame.CanDraw(firstDrawContext) &&
                placed == null)
            {
                var cardHandGO = spawnFunc();
                cardHandGO.transform.SetPositionAndRotation(
                    topCardTransform.position, topCardTransform.rotation
                );
                cardHandGO.transform.SetParent(transform);
                placed = cardHandGO;
            }
            hoverInteractable.transform.SetPositionAndRotation(
                topCardTransform.position, topCardTransform.rotation
            );

        }

        private void HandInteractableThrowHandler(GameObject sender)
        {
            if (sender == placed) placed = null;

            PlaceOnTop(deckGraphic.TopCardTransform, SpawnInteractable);
        }

        private void HandleInteractableDeath(GameObject sender)
        {
            var instance = sender as GameObject;
            var projectile = instance.GetComponent<HandProjectile>();
            projectile.OnThrow.RemoveListener(HandInteractableThrowHandler);
            projectile.OnKilled.RemoveListener(HandleInteractableDeath);
        }

        private GameObject SpawnInteractable()
        {
            GameObject spawned = handInteractableSpawner.Spawn();
            HandProjectile projectile = spawned.GetComponent<HandProjectile>();
            projectile.OnThrow.AddListener(HandInteractableThrowHandler);
            projectile.OnKilled.AddListener(HandleInteractableDeath);
            ControllerSwapper swapper = spawned.GetComponent<ControllerSwapper>();
            swapper.SetController(ControllerTypes.PLAYER);
            return spawned;
        }
    }
}
