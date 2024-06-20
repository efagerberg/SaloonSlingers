using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

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
        [FormerlySerializedAs("OnDamaged")]
        public UnityEvent<GameObject, IReadOnlyAttribute> Damaged = new();

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

            CardHand handProjectile = collidingObject.GetComponent<CardHand>();
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
            Damaged.Invoke(gameObject, HitPoints);
        }
    }
}
