using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class PickupOnCollision : MonoBehaviour
    {
        [SerializeField]
        private Pickup pickup;

        private void OnCollisionEnter(Collision collision)
        {
            pickup.TryPickup(collision.gameObject);
        }
    }
}
