using System;
using System.Collections.Generic;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class EnemyHandInteractableController : MonoBehaviour
    {
        public IList<Card> Cards { get => handProjectile.Cards; }

        private HandProjectile handProjectile;
        private Rigidbody rb;

        public void Draw(Deck deck, Func<GameObject> spawn)
        {
            if (Cards.Count == 0)
            {
                handProjectile.AssignDeck(deck);
                handProjectile.Pickup(spawn);
            }
            else
                handProjectile.TryDrawCard(spawn);
        }

        public void Throw(Vector3 velocity)
        {
            handProjectile.ToggleCommitHand();
            transform.Rotate(-90, 0, 0);
            handProjectile.Throw();
            rb.AddTorque(velocity.magnitude * transform.up, ForceMode.VelocityChange);
            rb.AddForce(velocity, ForceMode.VelocityChange);
            handProjectile.gameObject.layer = LayerMask.NameToLayer("EnemyProjectile");
        }

        private void Awake()
        {
            handProjectile = GetComponent<HandProjectile>();
            rb = GetComponent<Rigidbody>();
        }
    }
}
