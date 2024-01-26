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
                DamageMode.HAND_VALUE => collidingObject.GetComponent<HandProjectile>().HandEvaluation.Score,
                _ => 0
            };
            HitPoints ??= GetComponent<Attributes>().Registry[AttributeType.Health];
            HitPoints.Decrease(damage);
        }
    }
}