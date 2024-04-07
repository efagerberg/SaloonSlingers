using SaloonSlingers.Unity.Actor;

using UnityEngine;
using UnityEngine.Events;

namespace SaloonSlingers.Unity
{
    public class HandProjectileCurseTarget : MonoBehaviour
    {
        public HandProjectile Cursed { get; private set; }
        public UnityEvent<GameObject, HandProjectile> OnCursed = new();

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

            Cursed = handProjectile;
            // Melee cards can still change their mode, this ensures we do not keep cursed the target
            // if the player changes to damage mode or comes out of it because of some restriction like
            // only 1 card hands can be cursed cards for example.
            Cursed.OnDamageMode.AddListener(RemoveCurse);
            OnCursed.Invoke(gameObject, Cursed);
        }

        private void RemoveCurse(GameObject sender)
        {
            if (Cursed == null) return;

            Cursed.OnDamageMode.RemoveListener(RemoveCurse);
            Cursed = null;
        }
    }
}