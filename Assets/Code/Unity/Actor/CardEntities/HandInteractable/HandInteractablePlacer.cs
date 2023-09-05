using System;

using UnityEngine;

namespace SaloonSlingers.Unity.CardEntities
{
    public class HandInteractablePlacer : MonoBehaviour
    {
        private DeckGraphic deckGraphic;
        private HandInteractableSpawner handInteractableSpawner;
        private GameObject placed;

        private void Start()
        {
            handInteractableSpawner = GameObject.FindGameObjectWithTag("HandInteractableSpawner")
                                                .GetComponent<HandInteractableSpawner>();

            deckGraphic = GetComponent<DeckGraphic>();
            if (!deckGraphic.CanDraw) return;

            PlaceOnTop(deckGraphic.TopCardTransform, SpawnInteractable());
        }

        private void PlaceOnTop(Transform topCardTransform, GameObject cardHandGO)
        {
            cardHandGO.transform.SetPositionAndRotation(
                topCardTransform.position, topCardTransform.rotation
            );
            cardHandGO.transform.SetParent(transform);
            placed = cardHandGO;
        }

        private void HandInteractableHeldHandler(HandProjectile sender, EventArgs _)
        {
            if (sender.gameObject == placed) placed = null;

            if (!deckGraphic.CanDraw || placed) return;

            PlaceOnTop(deckGraphic.TopCardTransform, SpawnInteractable());
        }

        private void HandleInteractableDeath(object sender, EventArgs _)
        {
            var instance = sender as GameObject;
            var projectile = instance.GetComponent<HandProjectile>();
            foreach (ICardGraphic c in projectile.CardGraphics)
                c.Die();
            projectile.OnHandProjectileHeld -= HandInteractableHeldHandler;
            projectile.Death -= HandleInteractableDeath;
        }

        private GameObject SpawnInteractable()
        {
            GameObject spawned = handInteractableSpawner.Spawn();
            HandProjectile projectile = spawned.GetComponent<HandProjectile>();
            projectile.OnHandProjectileHeld += HandInteractableHeldHandler;
            projectile.Death += HandleInteractableDeath;
            ControllerSwapper swapper = spawned.GetComponent<ControllerSwapper>();
            swapper.SetController(ControllerTypes.PLAYER);
            return spawned;
        }
    }
}
