using System;

using UnityEngine;
using UnityEngine.Pool;

using SaloonSlingers.Unity.CardEntities;

namespace SaloonSlingers.Unity
{
    public class HandInteractablePlacer : MonoBehaviour
    {
        [SerializeField]
        private GameObject handInteractablePrefab;
        [SerializeField]
        private int poolSize = 5;

        private IObjectPool<GameObject> handInteractablePool;
        private DeckGraphic deckGraphic;

        private void Start()
        {
            handInteractablePool = new ObjectPool<GameObject>(
                () => {
                    GameObject go = Instantiate(handInteractablePrefab);
                    CardHand cardHand = go.GetComponent<CardHand>();
                    cardHand.OnHandInteractableHeld += HandInteractableHeldHandler;
                    cardHand.OnHandInterableReadyToRespawn += HandInteractableReadyToRespawn;
                    go.SetActive(false);
                    return go;
                },
                (GameObject go) => go.SetActive(true),
                (GameObject go) => go.SetActive(false),
                (GameObject go) => {
                    CardHand cardHand = go.GetComponent<CardHand>();
                    cardHand.OnHandInteractableHeld -= HandInteractableHeldHandler;
                    cardHand.OnHandInterableReadyToRespawn -= HandInteractableReadyToRespawn;
                },
                defaultCapacity: poolSize
            );
            deckGraphic = GetComponent<DeckGraphic>();
            GameObject first = handInteractablePool.Get();
            PlaceOnTop(deckGraphic.TopCardTransform, first);
        }

        private void PlaceOnTop(Transform topCardTransform, GameObject cardHandGO)
        {
            cardHandGO.transform.SetPositionAndRotation(
                topCardTransform.position, topCardTransform.rotation
            );
            cardHandGO.transform.SetParent(transform);
        }

        private void HandInteractableHeldHandler(CardHand sender, EventArgs _)
        {
            sender.transform.SetParent(null);
            PlaceOnTop(deckGraphic.TopCardTransform, handInteractablePool.Get());
        }

        private void HandInteractableReadyToRespawn(CardHand sender, EventArgs _)
        {
            handInteractablePool.Release(sender.gameObject);
        }
    }
}
