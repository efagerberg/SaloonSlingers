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
        private float totalCardDegrees;
        private GameObject handPanel;
        private CardSpawner cardSpawner;
        private float handCanvasUncommittedSize;
        private float handCanvasCommittedSize;
        private List<TangibleCard> tangibleCards;
        private bool handCommitted;
        private ISlingerAttributes slingerAttributes;

        public HandLayoutMediator(GameObject slingerGameObject, GameObject handPanel, CardSpawner cardSpawner, float totalCardDegrees)
        {
            tangibleCards = new List<TangibleCard>();
            var slinger = slingerGameObject.GetComponent<ISlinger>();
            slingerAttributes = slinger.Attributes;
            RectTransform canvasTransform = handPanel.transform.parent.GetComponent<RectTransform>();
            handCanvasUncommittedSize = canvasTransform.rect.width;
            this.totalCardDegrees = totalCardDegrees;
            this.cardSpawner = cardSpawner;
            this.handPanel = handPanel;
        }

        public void ApplyLayoutRotation()
        {
            tangibleCards.Zip(
                HandRotationCalculator.CalculateRotations(tangibleCards.Count, totalCardDegrees),
                (first, second) => (first, second)
            ).Select(r => ApplyLayoutRotationToCard(r.second, r.first)).ToList();
        }

        public void ToggleCommitHand(InputAction.CallbackContext _)
        {
            handCommitted = !handCommitted;
            var canvasTransform = handPanel.transform.parent.GetComponent<RectTransform>();
            float newCanvasWidth;

            if (handCommitted)
            {
                newCanvasWidth = handCanvasCommittedSize;
                tangibleCards.ForEach(RevertLayoutRotationOfCard);
            }
            else
            {
                newCanvasWidth = handCanvasUncommittedSize;
                ApplyLayoutRotation();
            }
            canvasTransform.sizeDelta = new Vector2(newCanvasWidth, canvasTransform.sizeDelta.y);
        }

        public void AddCardToHand(GameRulesManager gameRulesManager)
        {
            if (handCommitted || slingerAttributes.Hand.Count() >= gameRulesManager.GameRules.MaxHandSize)
                return;

            Card card = slingerAttributes.Deck.Dequeue();
            slingerAttributes.Hand.Add(card);
            TangibleCard tangibleCard = cardSpawner.Spawn(card);
            if (handCanvasCommittedSize == 0)
                handCanvasCommittedSize = tangibleCard.GetComponent<RectTransform>().rect.width;
            tangibleCard.transform.position = new Vector3(0, 0, (tangibleCards.Count() - 1) * zOffset);
            tangibleCard.transform.SetParent(handPanel.transform, false);
            tangibleCards.Add(tangibleCard);
            ApplyLayoutRotation();
        }

        public void Dispose()
        {
            for (int i = 0; i < tangibleCards.Count(); i++)
            {
                TangibleCard tangibleCard = tangibleCards[i];
                cardSpawner.Despawn(tangibleCard);
                tangibleCards.RemoveAt(i);
                slingerAttributes.Hand.RemoveAt(i);
            }
        }

        private static TangibleCard ApplyLayoutRotationToCard(float degrees, TangibleCard tangibleCard)
        {
            RevertLayoutRotationOfCard(tangibleCard);
            Vector3[] corners = new Vector3[4];
            tangibleCard.GetComponent<RectTransform>().GetLocalCorners(corners);
            tangibleCard.transform.RotateAround(corners[3], tangibleCard.transform.forward, degrees);
            return tangibleCard;
        }

        private static void RevertLayoutRotationOfCard(TangibleCard current)
        {
            Transform currentTransform = current.transform;
            currentTransform.localRotation = Quaternion.Euler(new Vector3(
                currentTransform.localEulerAngles.x,
                currentTransform.localEulerAngles.y,
                // Reset z rotation to what it was before
                currentTransform.parent.localEulerAngles.z
            ));
        }
    }
}
