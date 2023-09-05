using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace SaloonSlingers.Unity.CardEntities
{
    public class HandLayoutMediator
    {
        private const float zOffset = -0.001f;
        private readonly RectTransform handPanelRectTransform;
        private readonly float originalPanelWidth;
        private readonly IList<ICardGraphic> cardGraphics;

        public HandLayoutMediator(RectTransform handPanelRectTransform)
        {
            cardGraphics = new List<ICardGraphic>();
            this.handPanelRectTransform = handPanelRectTransform;
            originalPanelWidth = handPanelRectTransform.rect.width;
        }

        public void ApplyLayout(bool isHandCommitted, Func<int, IEnumerable<float>> rotationCalculator)
        {
            float newCanvasWidth;

            if (isHandCommitted)
            {
                newCanvasWidth = 0;
                foreach (ICardGraphic x in cardGraphics)
                {
                    x.transform.SetLocalPositionAndRotation(
                        GetRevertedLocalPosition(x.transform),
                        GetRevertedLocalRotation(x.transform)
                    );
                }
            }
            else
            {
                newCanvasWidth = originalPanelWidth;
                ApplyLayoutRotation(rotationCalculator);
            }
            handPanelRectTransform.sizeDelta = new Vector2(newCanvasWidth, handPanelRectTransform.sizeDelta.y);
        }

        public void AddCardToLayout(ICardGraphic cardGraphic, Func<int, IEnumerable<float>> rotationCalculator)
        {
            cardGraphic.transform.SetParent(handPanelRectTransform, false);
            cardGraphic.transform.localPosition = new(0, 0, (cardGraphics.Count() - 1) * zOffset);
            cardGraphics.Add(cardGraphic);
            ApplyLayoutRotation(rotationCalculator);
        }

        public void Reset()
        {
            for (int i = cardGraphics.Count() - 1; i >= 0; i--)
            {
                ICardGraphic cardGraphic = cardGraphics[i];
                cardGraphic.transform.SetLocalPositionAndRotation(
                    GetRevertedLocalPosition(cardGraphic.transform),
                    GetRevertedLocalRotation(cardGraphic.transform)
                );
                cardGraphics.RemoveAt(i);
            }
            handPanelRectTransform.sizeDelta = new Vector2(originalPanelWidth, handPanelRectTransform.sizeDelta.y);
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
