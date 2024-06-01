using System;
using System.Collections.Generic;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class EnemyHandInteractableController : MonoBehaviour
    {
        public IReadOnlyCollection<Card> Cards { get => handProjectile.Cards; }

        private HandProjectile handProjectile;
        private Rigidbody rb;

        public void Draw(Deck deck, IReadOnlyDictionary<AttributeType, Core.Attribute> attributeRegistry, Func<GameObject> spawn)
        {
            if (Cards.Count == 0)
            {
                handProjectile.Assign(deck, attributeRegistry);
                handProjectile.Pickup(spawn, GameManager.Instance.Saloon.HouseGame);
                handProjectile.gameObject.layer = LayerMask.NameToLayer("EnemyProjectile");
            }
            else
                handProjectile.TryDrawCard(spawn, GameManager.Instance.Saloon.HouseGame);
        }

        public void Throw(Vector3 velocity)
        {
            transform.Rotate(-90, 0, 0);
            handProjectile.Throw();
            rb.AddTorque(velocity.magnitude * transform.up, ForceMode.VelocityChange);
            rb.AddForce(velocity, ForceMode.VelocityChange);
        }

        private void Awake()
        {
            handProjectile = GetComponent<HandProjectile>();
            rb = GetComponent<Rigidbody>();
        }
    }
}
