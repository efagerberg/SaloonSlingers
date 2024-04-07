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
        public HandProjectile Cursed { get; private set; }

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

            HandProjectile handProjectile = collidingObject.GetComponent<HandProjectile>();
            switch (handProjectile.Mode)
            {
                case HandProjectileMode.Damage:
                    uint damage = DamageMode switch
                    {
                        DamageMode.DECREMENT => 1,
                        DamageMode.HAND_VALUE => handProjectile.HandEvaluation.Score,
                        _ => 0
                    };
                    attributes ??= GetComponent<Attributes>();
                    HitPoints ??= attributes?.Registry[AttributeType.Health];

                    if (HitPoints == null) return;

                    HitPoints.Decrease(damage);
                    break;
                case HandProjectileMode.Curse:
                    Cursed = handProjectile;
                    // Melee cards can still change their mode, this ensures we do not keep cursed the target
                    // if the player changes to damage mode or comes out of it because of some restriction like
                    // only 1 card hands can be cursed cards for example.
                    Cursed.OnDamageMode.AddListener(RemoveCurse);
                    break;
            }
        }

        private void RemoveCurse(GameObject sender)
        {
            if (Cursed == null) return;

            Cursed.OnDamageMode.RemoveListener(RemoveCurse);
            Cursed = null;
        }
    }
}