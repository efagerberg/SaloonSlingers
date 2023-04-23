using System.Collections.Generic;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.CardEntities;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class EnemyHandInteractableController : MonoBehaviour
    {
        public IList<Card> Cards { get => handProjectile.Cards; }

        [SerializeField]
        private float speed = 5f;

        private HandProjectile handProjectile;
        private Rigidbody rb;
        private Vector3 throwDirection;

        public void Draw(Deck deck, ICardSpawner cardSpawner)
        {
            handProjectile.AssignDeck(deck);
            if (Cards.Count == 0)
                handProjectile.Pickup(cardSpawner.Spawn);
            else
                handProjectile.TryDrawCard(cardSpawner.Spawn);
        }

        public void Throw(Vector3 throwDirection)
        {
            handProjectile.ToggleCommitHand();
            transform.Rotate(-90, 0, 0);
            handProjectile.Throw();
            this.throwDirection = throwDirection;
        }

        private void Start()
        {
            handProjectile = GetComponent<HandProjectile>();
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (handProjectile.IsThrown)
            {
                rb.AddTorque(speed * Time.fixedDeltaTime * transform.up);
                rb.AddForce(speed * Time.fixedDeltaTime * throwDirection);
            }
        }
    }
}
