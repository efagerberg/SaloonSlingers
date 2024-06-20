using System;
using System.Collections.Generic;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class EnemyHandInteractableController : MonoBehaviour
    {
        public IReadOnlyCollection<Card> Cards { get => hand.Cards; }

        private CardHand hand;
        private Projectile projectile;

        public void Draw(Deck deck, IReadOnlyDictionary<AttributeType, Core.Attribute> attributeRegistry, Func<GameObject> spawn)
        {
            if (Cards.Count == 0)
            {
                hand.Assign(deck, attributeRegistry);
                hand.Pickup(spawn, GameManager.Instance.Saloon.HouseGame);
                hand.gameObject.layer = LayerMask.NameToLayer("EnemyInteractable");
            }
            else
                hand.TryDrawCard(spawn, GameManager.Instance.Saloon.HouseGame);
        }

        public void Throw(Vector3 velocity)
        {
            projectile.Throw(velocity, velocity.magnitude * transform.up);
        }

        private void Awake()
        {
            hand = GetComponent<CardHand>();
            projectile = GetComponent<Projectile>();
        }
    }
}
