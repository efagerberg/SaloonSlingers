using System.Collections.Generic;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;
using UnityEngine.Events;

namespace SaloonSlingers.Unity
{
    public class HandProjectileCurseTarget : MonoBehaviour
    {
        public IReadOnlyCollection<Card> Cursed { get; private set; }
        public UnityEvent<GameObject, IReadOnlyCollection<Card>> OnCursed = new();

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

            HandProjectile handProjectile = collidingObject.GetComponent<HandProjectile>();

            if (handProjectile.Mode != HandProjectileMode.Curse) return;

            // Need to make a copy since we don't want the cards to change as the slinger
            // draws more cards;
            // This can happen when using the handprojectile as a melee weapon.
            Cursed = new List<Card>(handProjectile.Cards);
            OnCursed.Invoke(gameObject, Cursed);
        }
    }
}