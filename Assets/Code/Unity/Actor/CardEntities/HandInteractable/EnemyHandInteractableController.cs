using System;
using System.Collections.Generic;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class EnemyHandInteractableController : MonoBehaviour
    {
        public IReadOnlyCollection<Card> Cards { get => handProjectile.Cards; }

        private CardHand handProjectile;
        private Projectile projectile;

        public void Draw(Deck deck, IReadOnlyDictionary<AttributeType, Core.Attribute> attributeRegistry, Func<GameObject> spawn)
        {
            if (Cards.Count == 0)
            {
                handProjectile.Assign(deck, attributeRegistry);
                handProjectile.Pickup(spawn, GameManager.Instance.Saloon.HouseGame);
                handProjectile.gameObject.layer = LayerMask.NameToLayer("EnemyInteractable");
            }
            else
                handProjectile.TryDrawCard(spawn, GameManager.Instance.Saloon.HouseGame);
        }

        public void Throw(Vector3 velocity)
        {
            projectile.Throw(velocity, velocity.magnitude * transform.up);
        }

        private void Awake()
        {
            handProjectile = GetComponent<CardHand>();
            projectile = GetComponent<Projectile>();
        }
    }
}
