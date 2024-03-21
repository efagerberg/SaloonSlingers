using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;

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

        private Attributes attributes;

        private void OnCollisionEnter(Collision collision)
        {
            HandleCollision(collision.gameObject);
        }

        private void OnTriggerEnter(Collider collider)
        {
            HandleCollision(collider.gameObject);
        }

        private void HandleCollision(GameObject collidingObject)
        {
            if (!collidingObject.CompareTag("HandInteractable")) return;

            uint damage = DamageMode switch
            {
                DamageMode.DECREMENT => 1,
                DamageMode.HAND_VALUE => collidingObject.GetComponent<HandProjectileActor>().HandEvaluation.Score,
                _ => 0
            };
            attributes ??= GetComponent<Attributes>();
            HitPoints ??= attributes?.Registry[AttributeType.Health];

            if (HitPoints == null) return;

            HitPoints.Decrease(damage);
        }
    }
}