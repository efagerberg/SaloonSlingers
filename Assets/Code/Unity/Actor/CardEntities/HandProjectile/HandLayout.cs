using System;
using System.Collections.Generic;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class HandLayout : MonoBehaviour
    {
        [SerializeField]
        private RectTransform handPanelRectTransform;
        [SerializeField]
        private float totalCardDegrees = 30f;

        private HandLayoutMediator handLayoutMediator;
        private bool isStacked = false;
        private Func<int, IEnumerable<float>> cardRotationCalculator;

        public void Stack()
        {
            isStacked = true;
            handLayoutMediator.ApplyLayout(isStacked, cardRotationCalculator);
        }

        public void Unstack()
        {
            isStacked = false;
            handLayoutMediator.ApplyLayout(isStacked, cardRotationCalculator);
        }

        public void AddCard(HandProjectile sender, ICardGraphic card)
        {
            handLayoutMediator.AddCardToLayout(card, cardRotationCalculator);
        }

        private void Awake()
        {
            handLayoutMediator = new(handPanelRectTransform);
        }

        private void OnEnable()
        {
            cardRotationCalculator = (n) => HandRotationCalculator.CalculateRotations(n, totalCardDegrees);
        }
        private void OnDisable()
        {
            handLayoutMediator.Reset();
        }
    }
}
