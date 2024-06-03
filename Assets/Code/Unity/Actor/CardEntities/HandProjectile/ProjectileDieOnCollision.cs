using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class ProjectileDieOnCollision : MonoBehaviour
    {
        private Actor actor;
        private Rigidbody rigidBody;

        public void HandleCollision(GameObject contactingObject)
        {
            var antagonistLayer = LayerMask.LayerToName(gameObject.layer) switch
            {
                "PlayerProjectile" => LayerMask.NameToLayer("Enemy"),
                "EnemyProjectile" => LayerMask.NameToLayer("Player"),
                _ => -1,
            };
            var collidingWithAntagnoist = antagonistLayer != -1 && contactingObject.layer == antagonistLayer;
            var isSelfLethal = (
                collidingWithAntagnoist ||
                (!rigidBody.isKinematic &&
                 contactingObject.layer != LayerMask.NameToLayer("Environment") &&
                 contactingObject.layer != LayerMask.NameToLayer("Hand"))
            );
            if (!isSelfLethal) return;

            actor.Kill(delay: true);
        }

        public void HandleCollision(Collision other)
        {
            HandleCollision(other.gameObject);
        }

        public void HandleCollision(Collider other)
        {
            HandleCollision(other.gameObject);
        }

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
            actor = GetComponent<Actor>();
        }
    }
}
