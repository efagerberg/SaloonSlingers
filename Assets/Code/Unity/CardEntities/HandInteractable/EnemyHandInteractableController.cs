using System.Collections.Generic;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.CardEntities;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class EnemyHandInteractableController : MonoBehaviour
    {
        public IList<Card> Cards { get => handProjectile.Cards; }

        private HandProjectile handProjectile;
        private Rigidbody rb;

        public void Draw(Deck deck, ICardSpawner cardSpawner)
        {
            if (Cards.Count == 0)
            {
                handProjectile.AssignDeck(deck);
                handProjectile.Pickup(cardSpawner.Spawn);
                handProjectile.gameObject.layer = LayerMask.NameToLayer("EnemyProjectile");
            }
            else
                handProjectile.TryDrawCard(cardSpawner.Spawn);
        }

        public void Throw(Vector3 velocity)
        {
            handProjectile.ToggleCommitHand();
            transform.Rotate(-90, 0, 0);
            handProjectile.Throw();
            rb.AddTorque(velocity.magnitude * transform.up, ForceMode.VelocityChange);
            rb.AddForce(velocity, ForceMode.VelocityChange);
        }

        private void Start()
        {
            handProjectile = GetComponent<HandProjectile>();
            rb = GetComponent<Rigidbody>();
        }
    }
}
