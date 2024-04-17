using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;
using UnityEngine.Events;

namespace SaloonSlingers.Unity
{
    public enum DamageMode
    {
        DECREMENT,
        HAND_VALUE,
    }

    public class HandProjectileDamageTarget : MonoBehaviour
    {
        public Attribute HitPoints { get; set; }
        public DamageMode DamageMode = DamageMode.DECREMENT;
        public UnityEvent<GameObject, IReadOnlyAttribute> OnDamaged = new();

        private Attributes attributes;

        public void HandleCollision(Collision collision)
        {
            HandleCollision(collision.gameObject);
        }

        public void HandleCollision(Collider collider)
        {
            HandleCollision(collider.gameObject);
        }

        private void HandleCollision(GameObject collidingObject)
        {
            if (!collidingObject.CompareTag("HandInteractable")) return;

            HandProjectile handProjectile = collidingObject.GetComponent<HandProjectile>();
            if (handProjectile.Mode != HandProjectileMode.Damage) return;

            uint damage = DamageMode switch
            {
                DamageMode.DECREMENT => 1,
                DamageMode.HAND_VALUE => handProjectile.HandEvaluation.Score,
                _ => 0
            };
            attributes ??= GetComponent<Attributes>();
            HitPoints ??= attributes != null ? attributes.Registry[AttributeType.Health] : null;

            if (HitPoints == null) return;

            HitPoints.Decrease(damage);
            OnDamaged.Invoke(gameObject, HitPoints);
        }
    }
}
