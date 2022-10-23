using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace SaloonSlingers.Unity.CardEntities
{
    public class CardHandLayoutMediator
    {
        private const float zOffset = -0.001f;
        private readonly RectTransform handPanelRectTransform;
        private readonly RectTransform handCanvasRectTransform;
        private readonly float handCanvasUncommittedSize;
        private float handCanvasCommittedSize;
        private readonly IList<ICardGraphic> cardGraphics;

        public CardHandLayoutMediator(RectTransform handPanelRectTransform, RectTransform handCanvasRectTransform)
        {
            cardGraphics = new List<ICardGraphic>();
            this.handPanelRectTransform = handPanelRectTransform;
            this.handCanvasRectTransform = handCanvasRectTransform;
            handCanvasUncommittedSize = handCanvasRectTransform.rect.width;
        }

        public void ApplyLayout(bool isHandCommitted, Func<int, IEnumerable<float>> rotationCalculator)
        {
            float newCanvasWidth;

            if (isHandCommitted)
            {
                newCanvasWidth = handCanvasCommittedSize;
                foreach (ICardGraphic x in cardGraphics) {
                    x.transform.localRotation = GetRevertedLocalRotation(x.transform);
                    x.transform.localPosition = GetRevertedLocalPosition(x.transform);
                }
            }
            else
            {
                newCanvasWidth = handCanvasUncommittedSize;
                ApplyLayoutRotation(rotationCalculator);
            }
            handCanvasRectTransform.sizeDelta = new Vector2(newCanvasWidth, handCanvasRectTransform.sizeDelta.y);
        }

        public void AddCardToLayout(ICardGraphic cardGraphic, Func<int, IEnumerable<float>> rotationCalculator)
        {
            if (handCanvasCommittedSize == 0)
                handCanvasCommittedSize = cardGraphic.GetComponent<RectTransform>().rect.width;
            cardGraphic.transform.SetParent(handPanelRectTransform, false);
            cardGraphic.transform.localPosition = new Vector3(0, 0, (cardGraphics.Count() - 1) * zOffset);
            cardGraphics.Add(cardGraphic);
            ApplyLayoutRotation(rotationCalculator);
        }

        public void Dispose()
        {
            for (int i = cardGraphics.Count() - 1; i >= 0; i--)
            {
                ICardGraphic cardGraphic = cardGraphics[i];
                cardGraphic.transform.localRotation = GetRevertedLocalRotation(cardGraphic.transform);
                cardGraphic.transform.localPosition = GetRevertedLocalPosition(cardGraphic.transform);
                cardGraphic.transform.SetParent(null, false);
                cardGraphics.RemoveAt(i);
            }
        }

        private static void ApplyLayoutRotationToCard(float degrees, ICardGraphic cardGraphic)
        {
            cardGraphic.transform.localRotation = GetRevertedLocalRotation(cardGraphic.transform);
            Vector3[] corners = new Vector3[4];
            cardGraphic.GetComponent<RectTransform>().GetLocalCorners(corners);
            cardGraphic.transform.RotateAround(corners[3], -cardGraphic.transform.forward, degrees);
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

        private static Vector3 GetRevertedLocalPosition(Transform currentTransform)
        {
            return new Vector3(0, 0, currentTransform.localPosition.z);
        }

        private void ApplyLayoutRotation(Func<int, IEnumerable<float>> rotationCalculator)
        {
            cardGraphics.Zip(
                rotationCalculator(cardGraphics.Count),
                (first, second) => (first, second)
            ).ToList().ForEach(r => ApplyLayoutRotationToCard(r.second, r.first));
        }
    }
}
