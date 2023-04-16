using System;

using UnityEngine;
using UnityEngine.Pool;

namespace SaloonSlingers.Unity.CardEntities
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
                () =>
                {
                    GameObject go = Instantiate(handInteractablePrefab);
                    CardHand cardHand = go.GetComponent<CardHand>();
                    cardHand.OnHandInteractableHeld += HandInteractableHeldHandler;
                    cardHand.OnHandInteractableDied += HandInteractableDiedHandler;
                    go.SetActive(false);
                    return go;
                },
                (GameObject go) => go.SetActive(true),
                (GameObject go) => go.SetActive(false),
                (GameObject go) =>
                {
                    CardHand cardHand = go.GetComponent<CardHand>();
                    cardHand.OnHandInteractableHeld -= HandInteractableHeldHandler;
                    cardHand.OnHandInteractableDied -= HandInteractableDiedHandler;
                },
                defaultCapacity: poolSize
            );
            deckGraphic = GetComponent<DeckGraphic>();
            if (!deckGraphic.CanDraw) return;

            GameObject first = handInteractablePool.Get();
            PlaceOnTop(deckGraphic.TopCardTransform, first);
            deckGraphic.OnDeckGraphicEmpty += DeckGraphicEmptyHandler;
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
            if (!deckGraphic.CanDraw) return;

            PlaceOnTop(deckGraphic.TopCardTransform, handInteractablePool.Get());
        }

        private void HandInteractableDiedHandler(CardHand sender, EventArgs _)
        {
            foreach (ICardGraphic c in sender.Cards)
                deckGraphic.Despawn(c);
            handInteractablePool.Release(sender.gameObject);
        }

        private void DeckGraphicEmptyHandler(DeckGraphic sender, EventArgs e)
        {
            handInteractablePool.Clear();
        }

        private void OnEnable()
        {
            if (deckGraphic == null) return;
            deckGraphic.OnDeckGraphicEmpty += DeckGraphicEmptyHandler;
        }

        private void OnDisable()
        {
            if (deckGraphic == null) return;
            deckGraphic.OnDeckGraphicEmpty -= DeckGraphicEmptyHandler;
        }
    }
}
