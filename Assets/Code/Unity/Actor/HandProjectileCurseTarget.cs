using System.Collections.Generic;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace SaloonSlingers.Unity
{
    public class HandProjectileCurseTarget : MonoBehaviour
    {
        public IReadOnlyCollection<Card> Cards { get; private set; }
        [FormerlySerializedAs("OnCursed")]
        public UnityEvent<GameObject, IReadOnlyCollection<Card>> Cursed = new();

        public void HandleCollision(Collision collision)
        {
            HandleCollision(collision.gameObject);
        }

        public void HandleCollision(Collider collider)
        {
            HandleCollision(collider.gameObject);
        }

        public void HandleCollision(GameObject collidingObject)
        {
            if (!collidingObject.CompareTag("HandInteractable")) return;

            CardHand handProjectile = collidingObject.GetComponent<CardHand>();

            if (handProjectile.Mode != HandProjectileMode.Curse) return;

            // Need to make a copy since we don't want the cards to change as the slinger
            // draws more cards;
            // This can happen when using the handprojectile as a melee weapon.
            Cards = new List<Card>(handProjectile.Cards);
            Cursed.Invoke(gameObject, Cards);
        }

        private void OnDisable()
        {
            Cards = null;
        }
    }
}