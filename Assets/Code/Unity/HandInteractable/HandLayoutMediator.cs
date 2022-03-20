using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.InputSystem;

using SaloonSlingers.Core;
using SaloonSlingers.Core.SlingerAttributes;

namespace SaloonSlingers.Unity.HandInteractable
{
    public class HandLayoutMediator
    {
        private const float zOffset = -0.001f;
        private readonly RectTransform handPanelRectTransform;
        private readonly RectTransform handCanvasRectTransform;
        private readonly float handCanvasUncommittedSize;
        private float handCanvasCommittedSize;
        private readonly List<ITangibleCard> tangibleCards;
        private bool handCommitted;

        public HandLayoutMediator(RectTransform handPanelRectTransform, RectTransform handCanvasRectTransform)
        {
            tangibleCards = new List<ITangibleCard>();
            this.handPanelRectTransform = handPanelRectTransform;
            this.handCanvasRectTransform = handCanvasRectTransform;
            handCanvasUncommittedSize = handCanvasRectTransform.rect.width;
        }

        public (Vector2 canvasSizeDelta, IList<ITangibleCard> tangibleCards) ToggleCommitHand(Func<int, IEnumerable<float>> rotationCalculator, InputAction.CallbackContext _)
        {
            handCommitted = !handCommitted;
            float newCanvasWidth;

            if (handCommitted)
            {
                newCanvasWidth = handCanvasCommittedSize;
                tangibleCards.ForEach((t) => t.transform.localRotation = GetRevertedLocalRotation(t.transform));
            }
            else
            {
                newCanvasWidth = handCanvasUncommittedSize;
                ApplyLayoutRotation(rotationCalculator);
            }
            return (
                new Vector2(newCanvasWidth, handCanvasRectTransform.sizeDelta.y),
                tangibleCards
            );
        }

        public IList<ITangibleCard> AddCardToHand(int maxHandSize, ISlingerAttributes slingerAttributes, Func<Card, ITangibleCard> cardSpawner, Func<int, IEnumerable<float>> rotationCalculator)
        {
            if (handCommitted || slingerAttributes.Hand.Count() >= maxHandSize)
                return tangibleCards;

            Card card = slingerAttributes.Deck.Dequeue();
            slingerAttributes.Hand.Add(card);
            ITangibleCard tangibleCard = cardSpawner(card);
            if (handCanvasCommittedSize == 0)
                handCanvasCommittedSize = tangibleCard.GetComponent<RectTransform>().rect.width;
            tangibleCard.transform.position = new Vector3(0, 0, (tangibleCards.Count() - 1) * zOffset);
            tangibleCard.transform.SetParent(handPanelRectTransform, false);
            tangibleCards.Add(tangibleCard);
            return ApplyLayoutRotation(rotationCalculator);
        }

        public IList<ITangibleCard> Dispose(Action<ITangibleCard> cardDespawner, IList<Card> hand)
        {
            for (int i = tangibleCards.Count() - 1; i >= 0; i--)
            {
                ITangibleCard tangibleCard = tangibleCards[i];
                cardDespawner(tangibleCard);
                tangibleCards.RemoveAt(i);
                hand.RemoveAt(i);
            }
            return tangibleCards;
        }

        private static ITangibleCard ApplyLayoutRotationToCard(float degrees, ITangibleCard tangibleCard)
        {
            tangibleCard.transform.localRotation = GetRevertedLocalRotation(tangibleCard.transform);
            Vector3[] corners = new Vector3[4];
            tangibleCard.GetComponent<RectTransform>().GetLocalCorners(corners);
            tangibleCard.transform.RotateAround(corners[3], tangibleCard.transform.forward, degrees);
            return tangibleCard;
        }

        private static Quaternion GetRevertedLocalRotation(Transform currentTransform)
        {
            return Quaternion.Euler(new Vector3(
                currentTransform.localEulerAngles.x,
                currentTransform.localEulerAngles.y,
                // Reset z rotation to what it was before
                currentTransform.parent.localEulerAngles.z
            ));
        }

        private IList<ITangibleCard> ApplyLayoutRotation(Func<int, IEnumerable<float>> rotationCalculator)
        {
            return tangibleCards.Zip(
                rotationCalculator(tangibleCards.Count),
                (first, second) => (first, second)
            ).Select(r => ApplyLayoutRotationToCard(r.second, r.first)).ToList();
        }
    }
}
