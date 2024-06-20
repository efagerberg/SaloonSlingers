using System;

using UnityEngine;
using UnityEngine.Events;

namespace SaloonSlingers.Unity.Actor
{
    public enum StatusType
    {
        Damage = 0,
        Curse = 1
    }

    public class CardHandStatusType : MonoBehaviour
    {
        public UnityEvent<CardHandStatusType> CollisionEffectChanged = new();

        public StatusType Current
        {
            get => current;
            set
            {
                if (value == current) return;

                var canCurse = value == StatusType.Curse && hand.Cards.Count == 1;
                if (!canCurse && value == StatusType.Curse)
                    return;

                current = value;
                CollisionEffectChanged.Invoke(this);
            }
        }

        private StatusType current;
        private CardHand hand;

        private void Awake()
        {
            hand = GetComponent<CardHand>();
        }

        private void OnEnable()
        {
            hand.Drawn.AddListener(OnDrawn);
        }

        private void OnDisable()
        {
            hand.Drawn.RemoveListener(OnDrawn);
        }

        private void OnDrawn(CardHand sender, ICardGraphic spawned)
        {
            Current = StatusType.Damage;
        }
    }
}
